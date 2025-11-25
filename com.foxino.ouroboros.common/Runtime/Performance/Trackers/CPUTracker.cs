using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ouroboros.Common.Performance.Trackers
{
    public class CPUTracker : StatsTracker
    {
        public const string KeyName = "CPU";

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

            process = new Process();
            process.StartInfo.FileName = "typeperf";
            process.StartInfo.Arguments = string.Format("\"\\Process({0})\\% Processor Time\"", procName);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += (sender, args) =>
            {
                if (args == null || args.Data == null) return;

                var match = rgx.Match(args.Data);

                if (match.Success)
                {
                    float cpuLoadAllCores;
                    if (float.TryParse(match.Groups[1].ToString(), out cpuLoadAllCores))
                    {
                        int cpuLoad = Mathf.RoundToInt(cpuLoadAllCores / processorCount);

                        SetCurrent(cpuLoad);
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