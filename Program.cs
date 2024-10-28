﻿using System;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace SystemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            // 获取 CPU 型号
            string cpuInfo = GetCpuInfo();
            Console.WriteLine($"CPU 型号: {cpuInfo}");

            // 获取内存大小
            long memorySize = GetMemorySize();
            Console.WriteLine($"内存大小: {memorySize / (1024 * 1024)} MB");

            // 获取磁盘容量
            GetDiskInfo();

            Console.ReadLine();
        }

        static string GetCpuInfo()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["Name"].ToString();
                }
            }
            return "无法获取 CPU 信息";
        }

        static long GetMemorySize()
        {
            long totalMemory = 0;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select TotalVisibleMemorySize from Win32_OperatingSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalMemory = Convert.ToInt64(obj["TotalVisibleMemorySize"]);
                }
            }
            return totalMemory;
        }

        static void GetDiskInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    Console.WriteLine($"磁盘 {drive.Name}: {drive.TotalSize / (1024 * 1024)} MB 可用: {drive.AvailableFreeSpace / (1024 * 1024)} MB");
                }
            }
        }
    }
}
