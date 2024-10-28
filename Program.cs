using System;
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
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    cpuInfo = obj["Name"].ToString();
                }
            }
            return cpuInfo;
        }

        static long GetTotalPhysicalMemory()
        {
            long totalMemory = 0;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Capacity from Win32_PhysicalMemory"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalMemory += Convert.ToInt64(obj["Capacity"]);
                }
            }
            return totalMemory / (1024 * 1024); // 转换为 MB
        }

        static void GetDiskInfo()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select DeviceID, Size, FreeSpace from Win32_LogicalDisk"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string deviceId = obj["DeviceID"].ToString();
                    long size = Convert.ToInt64(obj["Size"]);
                    long freeSpace = Convert.ToInt64(obj["FreeSpace"]);
                    long usedSpace = size - freeSpace;

                    Console.WriteLine($"磁盘 {deviceId} - 总容量: {size / (1024 * 1024)} MB, 可用空间: {freeSpace / (1024 * 1024)} MB, 已用空间: {usedSpace / (1024 * 1024)} MB");
                }
            }
        }
    }
}
