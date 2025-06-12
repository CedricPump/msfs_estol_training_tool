from flask import Flask, Response, jsonify
from influxdb_client import InfluxDBClient
from influx_token import INFLUX_TOKEN
import xml.etree.ElementTree as ET
import datetime

# Config
INFLUX_URL = "https://eu-central-1-1.aws.cloud2.influxdata.com/"
INFLUX_ORG = "steffieth"
INFLUX_BUCKET = "My_eSTOL_Bucket"

app = Flask(__name__)

QUERY = f'''
from(bucket: "{INFLUX_BUCKET}")
  |> range(start: -24h)
  |> filter(fn: (r) => r._measurement == "stol_results")
  |> pivot(rowKey: ["_time", "User"], columnKey: ["_field"], valueColumn: "_value")
  |> sort(columns: ["_time"], desc: true)
  |> limit(n:1)
'''

def fetch_latest_score():
    with InfluxDBClient(url=INFLUX_URL, token=INFLUX_TOKEN, org=INFLUX_ORG) as client:
        query_api = client.query_api()
        tables = query_api.query(QUERY)

        for table in tables:
            for record in table.records:
                return record.values
    return None

@app.route("/score.xml")
def score_xml():
    latest = fetch_latest_score()
    if not latest:
        return Response(status=204)

    record = ET.Element("record")
    ET.SubElement(record, "time").text = latest["_time"].isoformat() if isinstance(latest["_time"], datetime.datetime) else str(latest["_time"])
    ET.SubElement(record, "user").text = str(latest.get("User", "unknown"))
    ET.SubElement(record, "score").text = str(latest.get("Score", "N/A"))
    ET.SubElement(record, "takeoff").text = str(latest.get("Takeoffdist", "N/A"))
    ET.SubElement(record, "landing").text = str(latest.get("Landingdist", "N/A"))


    xml_data = ET.tostring(record, encoding="utf-8", xml_declaration=True)
    return Response(xml_data, mimetype="application/xml")

@app.route("/score.json")
def score_json():
    latest = fetch_latest_score()
    if not latest:
        return jsonify({"error": "No data found"}), 204

    return jsonify({
        "time": latest["_time"].isoformat() if isinstance(latest["_time"], datetime.datetime) else str(latest["_time"]),
        "user": latest.get("User", "unknown"),
        "score": latest.get("Score", "N/A"),
        "takeoff": latest.get("Takeoffdist", "N/A"),
        "landing": latest.get("Landingdist", "N/A")
    })

if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(description="eSTOL Score Server")
    parser.add_argument("--port", type=int, default=80, help="Port to run the server on")
    args = parser.parse_args()

    app.run(port=args.port)
