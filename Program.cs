using System;
using Microsoft.Management.Infrastructure;

namespace SystemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            // 获取 CPU 型号
            string cpuInfo = GetCpuInfo();
            Console.WriteLine($"CPU 型号: {cpuInfo}");

            // 获取总内存大小
            long memorySize = GetTotalPhysicalMemory();
            Console.WriteLine($"内存大小: {memorySize} MB");

            // 获取磁盘容量
            GetDiskInfo();

            // 等待用户输入，防止窗口关闭
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        static string GetCpuInfo()
        {
            string cpuInfo = string.Empty;
            using (var cimSession = CimSession.Create("localhost"))
            {
                var instances = cimSession.QueryInstances(@"root\cimv2", "WQL", "SELECT Name FROM Win32_Processor");
                foreach (var instance in instances)
                {
                    cpuInfo = instance.CimInstanceProperties["Name"].Value.ToString();
                }
            }
            return cpuInfo;
        }

        static long GetTotalPhysicalMemory()
        {
            long totalMemory = 0;
            using (var cimSession = CimSession.Create("localhost"))
            {
                var instances = cimSession.QueryInstances(@"root\cimv2", "WQL", "SELECT Capacity FROM Win32_PhysicalMemory");
                foreach (var instance in instances)
                {
                    totalMemory += Convert.ToInt64(instance.CimInstanceProperties["Capacity"].Value);
                }
            }
            return totalMemory / (1024 * 1024); // 转换为 MB
        }

        static void GetDiskInfo()
        {
            using (var cimSession = CimSession.Create("localhost"))
            {
                var instances = cimSession.QueryInstances(@"root\cimv2", "WQL", "SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk");
                foreach (var instance in instances)
                {
                    string deviceId = instance.CimInstanceProperties["DeviceID"].Value.ToString();
                    long size = Convert.ToInt64(instance.CimInstanceProperties["Size"].Value);
                    long freeSpace = Convert.ToInt64(instance.CimInstanceProperties["FreeSpace"].Value);
                    long usedSpace = size - freeSpace;

                    Console.WriteLine($"磁盘 {deviceId} - 总容量: {size / (1024 * 1024)} MB, 可用空间: {freeSpace / (1024 * 1024)} MB, 已用空间: {usedSpace / (1024 * 1024)} MB");
                }
            }
        }
    }
}
