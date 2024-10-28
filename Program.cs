using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // 获取 CPU 信息
        Console.WriteLine("CPU 信息：");
        ExecuteCommand("wmic cpu get name");

        // 获取内存信息
        Console.WriteLine("\n内存信息：");
        ExecuteCommand("wmic MemoryChip get Capacity");

        // 获取磁盘信息
        Console.WriteLine("\n磁盘信息：");
        ExecuteCommand("wmic diskdrive get caption, mediaType, size");

        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
    }

    static void ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(processInfo))
        {
            using (var reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
                ConvertAndDisplayInfo(result, command);
            }
        }
    }

    static void ConvertAndDisplayInfo(string rawData, string command)
    {
        var lines = rawData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (command.Contains("cpu"))
        {
            // 处理 CPU 信息
            for (int i = 1; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    Console.WriteLine($"CPU 型号: {lines[i].Trim()}");
                }
            }
        }
        else if (command.Contains("MemoryChip"))
        {
            // 处理内存信息
            long totalMemory = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                if (long.TryParse(lines[i].Trim(), out long capacity))
                {
                    totalMemory += capacity; // 累加内存容量
                }
            }
            // 转换为 GB
            double totalMemoryGB = totalMemory / (1024.0 * 1024 * 1024);
            Console.WriteLine($"总内存: {totalMemoryGB:F2} GB");
        }
        else if (command.Contains("diskdrive"))
        {
            // 处理磁盘信息
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (columns.Length >= 3)
                {
                    string drive = string.Join(" ", columns, 0, columns.Length - 2); // 驱动器描述
                    long size = Convert.ToInt64(columns[columns.Length - 1]); // 总容量（字节）

                    // 转换为 GB
                    double sizeGB = size / (1024.0 * 1024 * 1024);
                    Console.WriteLine($"驱动器 {drive}: 总容量: {sizeGB:F2} GB");
                }
            }
        }
    }
}
