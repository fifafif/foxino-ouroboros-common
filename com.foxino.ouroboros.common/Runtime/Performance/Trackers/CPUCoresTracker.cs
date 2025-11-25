using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ouroboros.Common.Performance.Trackers
{
    public class CPUCoresTracker : StatsTrackerMulti
    {
        public const string KeyName = "CPU Cores";

        public override string Key { get { return KeyName; } }
        
        private Process process;
        private int processorCount;
        
        void OnDestroy()
        {
            StopProcess();
        }

        public override void StartTracking()
        {
            base.StartTracking();

            StartProcess();
        }

        public override void StopTracking()
        {
            base.StopTracking();

            StopProcess();
        }

        private void StartProcess()
        {
            processorCount = SystemInfo.processorCount;


            string pattern = "\\\"([0-9]+\\.[0-9]+)\\\"";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            string procName = Process.GetCurrentProcess().ProcessName;

            Data = new StatsTrackerData[processorCount];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < processorCount; ++i)
            {
                sb.AppendFormat("\"\\Processor({0})\\% Processor Time\" ", i);

                Data[i] = new StatsTrackerData();
            }

            process = new Process();
            process.StartInfo.FileName = "typeperf";
            process.StartInfo.Arguments = sb.ToString();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += (sender, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                {
                    return;
                }

                //UnityEngine.Debug.Log(args.Data);
                
                var match = rgx.Matches(args.Data);

                if (match.Count == processorCount)
                {
                    int i = 0;
                    foreach (Match m in match)
                    {
                        //UnityEngine.Debug.Log(m.Value);

                        float load;
                        
                        if (float.TryParse(m.Value.Trim('"'), out load))
                        {
                            //UnityEngine.Debug.Log("Load: " + i + "-" + load);

                            Data[i].SetCurrent(Mathf.RoundToInt(load));
                        }

                        ++i;
                    }
                }
            };

            process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log("error " + args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void StopProcess()
        {
            try
            {
                if (process != null
                    && !process.HasExited)
                {
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error during killing CPU stats process: " + e.Message);
            }
        }
    }
}