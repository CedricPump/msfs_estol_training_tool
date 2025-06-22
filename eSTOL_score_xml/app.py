from flask import Flask, Response, request
from influxdb_client import InfluxDBClient
from influx_token import INFLUX_TOKEN
import xml.etree.ElementTree as ET
import datetime
import argparse

# Config
INFLUX_URL = "https://eu-central-1-1.aws.cloud2.influxdata.com/"
INFLUX_ORG = "steffieth"
INFLUX_BUCKET = "My_eSTOL_Bucket"

app = Flask(__name__)

def build_score_query(session_key=None):
    session_filter = ""
    if session_key and session_key.lower() != "any":
        session_filter = f'|> filter(fn: (r) => r.SessionKey == "{session_key}")'
    else:
        session_filter = '|> filter(fn: (r) => exists r.SessionKey)'

    return f'''
from(bucket: "{INFLUX_BUCKET}")
  |> range(start: -24h)
  |> filter(fn: (r) => r._measurement == "stol_results")
  {session_filter}
  |> pivot(rowKey: ["_time", "User"], columnKey: ["_field"], valueColumn: "_value")
  |> group()
  |> sort(columns: ["_time"], desc: true)
  |> limit(n:1)
'''

def build_takeoff_query(session_key=None):
    session_filter = ""
    if session_key and session_key.lower() != "any":
        session_filter = f'|> filter(fn: (r) => r.SessionKey == "{session_key}")'

    return f'''
from(bucket: "{INFLUX_BUCKET}")
  |> range(start: -24h)
  |> filter(fn: (r) => r._measurement == "stol_event")
  {session_filter}
  |> filter(fn: (r) => r.EventType == "TAKEOFF")
  |> filter(fn: (r) => r._field == "Value")
  |> group()
  |> sort(columns: ["_time"], desc: true)
  |> limit(n:1)
'''




def fetch_latest_score(session_key=None):
    query = build_score_query(session_key)
    with InfluxDBClient(url=INFLUX_URL, token=INFLUX_TOKEN, org=INFLUX_ORG) as client:
        tables = client.query_api().query(query)
        for table in tables:
            for record in table.records:
                return record.values
    return None

def fetch_takeoff_events(session_key=None):
    query = build_takeoff_query(session_key)
    results = []
    with InfluxDBClient(url=INFLUX_URL, token=INFLUX_TOKEN, org=INFLUX_ORG) as client:
        tables = client.query_api().query(query)
        for table in tables:
            for record in table.records:
                results.append(record.values)
    return results

def build_score_xml_response(latest):
    def format_score(value):
        try:
            return "scratch" if float(value) < 0.01 else str(value)
        except (ValueError, TypeError):
            return str(value)

    records = ET.Element("records")
    record = ET.SubElement(records, "record")
    ET.SubElement(record, "time").text = latest["_time"].isoformat() if isinstance(latest["_time"], datetime.datetime) else str(latest["_time"])
    ET.SubElement(record, "user").text = str(latest.get("User", "unknown"))
    ET.SubElement(record, "score").text = format_score(latest.get("Score", "N/A"))
    ET.SubElement(record, "takeoff").text = str(latest.get("Takeoffdist", "N/A"))
    ET.SubElement(record, "landing").text = str(latest.get("Landingdist", "N/A"))
    ET.SubElement(record, "score_text").text = "total"
    ET.SubElement(record, "takeoff_text").text = "takeoff"
    ET.SubElement(record, "landing_text").text = "landing"

    xml_data = ET.tostring(records, encoding="utf-8", xml_declaration=True)
    return Response(xml_data, mimetype="application/xml")

def build_takeoff_xml_response(events):
    records = ET.Element("records")
    for event in events:
        record = ET.SubElement(records, "record")
        ET.SubElement(record, "time").text = event["_time"].isoformat() if isinstance(event["_time"], datetime.datetime) else str(event["_time"])
        ET.SubElement(record, "user").text = str(event.get("User", "unknown"))
        ET.SubElement(record, "value").text = str(event.get("_value", "N/A"))
        ET.SubElement(record, "takeoff_text").text = "takeoff"

    xml_data = ET.tostring(records, encoding="utf-8", xml_declaration=True)
    return Response(xml_data, mimetype="application/xml")

# Score path
@app.route("/<session_key>/score.xml")
def score_by_path(session_key):
    latest = fetch_latest_score(session_key)
    if not latest:
        empty_xml = ET.tostring(ET.Element("records"), encoding="utf-8", xml_declaration=True)
        return Response(empty_xml, mimetype="application/xml")

    return build_score_xml_response(latest)

@app.route("/score.xml")
def score_by_query():
    latest = fetch_latest_score("any")
    if not latest:
        empty_xml = ET.tostring(ET.Element("records"), encoding="utf-8", xml_declaration=True)
        return Response(empty_xml, mimetype="application/xml")

    return build_score_xml_response(latest)

# Takeoff path
@app.route("/<session_key>/takeoff.xml")
def takeoff_by_path(session_key):
    events = fetch_takeoff_events(session_key)
    if not events:
        empty_xml = ET.tostring(ET.Element("records"), encoding="utf-8", xml_declaration=True)
        return Response(empty_xml, mimetype="application/xml")

    return build_takeoff_xml_response(events)

@app.route("/takeoff.xml")
def takeoff_by_query():
    events = fetch_takeoff_events("any")
    if not events:
        empty_xml = ET.tostring(ET.Element("records"), encoding="utf-8", xml_declaration=True)
        return Response(empty_xml, mimetype="application/xml")

    return build_takeoff_xml_response(events)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="eSTOL Score Server")
    parser.add_argument("--port", type=int, default=80, help="Port to run the server on")
    args = parser.parse_args()

    app.run(host="0.0.0.0", port=args.port)
