﻿using System;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;


namespace eSTOL_Training_Tool
{
    public class Influx 
    {
        const string influxHost = "https://eu-central-1-1.aws.cloud2.influxdata.com/";
        const string bucket = "eSTOL2";
        const string org = "steffieth";
        const string token = "AAAA-BBBB-CCCC"; // InfluxDB 2.x requires an authentication token.

        InfluxDBClient influxDBClient;


        private static Influx instance;

        public static Influx GetInstance()
        {
            if (instance == null)
                instance = new Influx();

            return instance;
        }

        private Influx() 
        {
            influxDBClient = InfluxDBClientFactory.Create(influxHost, token);
        }

        public async void sendData(STOLResult stolResult) {
            var point = PointData.Measurement("stol_results")
                .Tag("User", stolResult.User)
                .Tag("planeType", stolResult.planeType)
                .Tag("preset", stolResult.preset == null ? "" :  stolResult.preset.title)
                .Field("Takeoffdist", stolResult.Takeoffdist)
                .Field("Touchdowndist", stolResult.Touchdowndist)
                .Field("Stoppingdist", stolResult.Stoppingdist)
                .Field("Landingdist", stolResult.Landingdist)
                .Field("TdPitch", stolResult.TdPitch)
                .Field("GrndSpeed", stolResult.GrndSpeed)
                .Field("VSpeed", stolResult.VSpeed)
                .Field("Score", stolResult.Score)
                .Field("PatternTime", stolResult.PatternTime.TotalSeconds)
                .Field("InitHash", stolResult.InitHash)
                .Timestamp(stolResult.time, WritePrecision.Ns);

            var writeApi = influxDBClient.GetWriteApiAsync();
            await writeApi.WritePointAsync(point, bucket, org);
        }

        public async void deletAll()
        {
            var start = DateTime.MinValue;
            var stop = DateTime.UtcNow;

            try
            {
                // Perform the delete operation
                var deleteApi = influxDBClient.GetDeleteApi();
                await deleteApi.Delete(start, stop, "", bucket, org);

                Console.WriteLine("All data from the bucket has been cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting data: {ex.Message}");
            }
        }
    }
}
