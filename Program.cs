using Brian_Moorman_TextFormatter;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Linq;
using System.Management;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Timers;

/// <summary>
/// Application to show a realtime update of system information
/// for all those times people ask me to help with their computers 
/// because someone with a CS degree must know computers /s

namespace Brian_Moorman_System_Stats
{
    class Program
    {


        static void Main(string[] args)
        {
            Backend be = new Backend();
            try
            {
                GetCPUInformation();
                GetGPUInformation();
                GetDiskDriveInformation();
                GetNetworkConnectionInformation();
                GetNetWorkInformationOther();
            }
            catch(Exception e)
            {
                be.SetTextColor(ConsoleColor.Red);
                Console.WriteLine("Error: {0}", e);
                be.DefaultTextColor();
            }
            Console.ReadLine();
            
            

            
        }
        #region CPU
        public static void GetCPUInformation()
        {
            Backend be = new Backend();
            be.TestPaddingFunction();
            ManagementObjectSearcher mOS = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject mObj in mOS.Get())
            {
                //get architecture number and return string descirption
                UInt16 archType = (UInt16)mObj["Architecture"];
                string archLabel = "";
                if (archType == 0)
                    archLabel = "x86 - 32bit ";
                if (archType == 1)
                    archLabel = "MIPS - Microprocessor without Interlocked Pipelined Stages";
                if (archType == 2)
                    archLabel = "Alpha - ALPHA AXP 64bit";
                if (archType == 3)
                    archLabel = "PowerPC 32bit";
                if (archType == 6)
                    archLabel = "ia64 - Intel Itanium Architecture 64bit";
                if (archType == 9)
                    archLabel = "x64 - 64bit ";

                //Header for CPU
                Console.WriteLine("\n---------------------------------------------------------------");
                be.SetTextColor(ConsoleColor.Green);
                Console.WriteLine(string.Format("{0}", be.StringFormatLeft("CPU Information", 36)));
                be.DefaultTextColor();
                Console.WriteLine("---------------------------------------------------------------\n\n");

                //CPU Data                
                string cpuName = mObj["Name"].ToString();
                
                //string cpuStatus = mObj["CPUStatus"].ToString();
                string loadPercent = mObj["LoadPercentage"].ToString();
                string printableLoadPercent = loadPercent + "%";
                string ExtClock = mObj["ExtClock"].ToString();
                string printableExtClock = ExtClock + "MHz";

                //CPU Arrays
                string[] cpuInformation = { cpuName, archLabel, printableLoadPercent, printableExtClock };
                string[] cpuLables = { "CPU: ", "Architecture: ", "Load Percent: ", "Clock Frequency: " };
                //TODO: Switch to List<T>
                //TODO: Colorize
                //Display information formatted
                for (int i = 0; i < cpuInformation.Length; i++)
                {
                    //https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting
                    //for element one: starting at 0 and aligns them into a 20 character field
                    //for element two: formats right with left being aligned
                    Console.WriteLine("{0, -20}" + "{1, -5}", cpuLables[i], cpuInformation[i]);
                }
            }
        }
        #endregion

        #region GPU

        public static void GetGPUInformation()
        {
            Backend be = new Backend();

            ManagementObjectSearcher mOS = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject gpuObj in mOS.Get())
            {
                //Header for GPU
                Console.WriteLine("\n---------------------------------------------------------------");
                be.SetTextColor(ConsoleColor.Green);
                Console.WriteLine(string.Format("{0}", be.StringFormatLeft("GPU Information", 36)));
                be.DefaultTextColor();
                Console.WriteLine("---------------------------------------------------------------\n\n");

                //get ram into noticable digits
                UInt32 adRam = (UInt32)gpuObj["AdapterRAM"];
                float actualRam = adRam;

                //Gpu Data
                string gpuName = gpuObj["Name"].ToString();
                string gpuID = gpuObj["DeviceID"].ToString();
                string gpuRam = (actualRam / (1073741824 / 2)).ToString("F");
                string printableRam = gpuRam + " GB";

                //Gpu Arrays
                string[] gpuInformation = { gpuName, gpuID, printableRam };
                string[] gpuLabels = { "GPU Name: ", "GPU ID: ", "GPU RAM: " };
                //Display information formatted
                for (int i = 0; i < gpuInformation.Length; i++)
                {
                    Console.WriteLine("{0, -20}" + "{1, -5}", gpuLabels[i], gpuInformation[i]);
                }
            }
        }

        #endregion

        #region Storage
        public static void GetDiskDriveInformation()
        {
            Backend be = new Backend();

            DriveInfo[] installedDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drives in installedDrives)
            {
                long freeSpace = (drives.TotalSize - drives.AvailableFreeSpace) / 1073741824;

                //Header for Disk Drive(s)
                Console.WriteLine("\n---------------------------------------------------------------");
                be.SetTextColor(ConsoleColor.Green);
                Console.WriteLine(string.Format("{0}", be.StringFormatLeft("Drive Information", 33)));
                be.DefaultTextColor();
                Console.WriteLine("---------------------------------------------------------------\n\n");

                //Drives Data              
                //Will print each drive into its own information section
                string driveName = drives.Name.ToString();
                string driveType = drives.DriveType.ToString();
                string totalSize = drives.TotalSize / 1073741824 + " GB";
                string availableSpace = drives.AvailableFreeSpace / 1073741824 + " GB";

                //Drive Arrays
                string[] driveLabels = { "Drive Name: ", "Drive Type: ", "Total Space: ", "Available Space: " };
                string[] driveInformation = { driveName, driveType, totalSize, availableSpace };

                for (int i = 0; i < driveInformation.Length; i++)
                {
                    Console.WriteLine("{0, -20 }" + "{1:-5}", driveLabels[i], driveInformation[i]);
                }

            }
        }
        #endregion

        #region NetworkConnection
        //returns blank
        public static void GetNetworkConnectionInformation()
        {
            Backend be = new Backend();
            Console.WriteLine("\n---------------------------------------------------------------");
            be.SetTextColor(ConsoleColor.Green);
            Console.WriteLine(string.Format("{0}", be.StringFormatLeft("Network Information", 36)));
            be.DefaultTextColor();
            Console.WriteLine("---------------------------------------------------------------\n\n");

            GetLocaliP();


        }
        #endregion

        #region IPAdress

        public static void GetLocaliP()
        {
            string hNAme = "Host Name: ";
            string[] networkLabels = { "IPv6 Address: ", "IPv6 Gateway: ", "IPv6 Mask: ",
                                           "IPv4 Address: ", "IPv4 Gateway: ", "IPv4 Mask"};
            var host = Dns.GetHostEntry(Dns.GetHostName());
            Console.WriteLine("{0, -20} {1:-5}", hNAme, host.HostName);
            IPAddress[] ipList = host.AddressList;
            string[] ipString = new string[ipList.Length];
            Console.WriteLine();
            for (int l = 0; l < ipList.Length; l++)
            {
                ipString[l] = ipList[l].ToString();
                if (ipList[0] == null)
                {
                    Console.WriteLine("No Connected Networks");
                    break;
                }
            }
            for (int i = 0; i < ipList.Length; i++)
            {
                Console.WriteLine("{0, -20}" + "{1:-5}", networkLabels[i].ToString(), ipString[i].ToString());
            }
        }
        #endregion

        #region Other Net Info

        public static void GetNetWorkInformationOther()
        {
            Backend be = new Backend();
            System.Management.ManagementClass wmiNetAdapter = new System.Management.ManagementClass("Win32_NetworkAdapter");
            System.Management.ManagementObject wmiNetAdapters = wmiNetAdapter;
            //Log.logInfo("Net adapters:" + wmiNetAdapters.get_Count());
            Console.WriteLine("\nNetwork Adapters: \n");
            foreach(var mO_1 in wmiNetAdapter.GetInstances())
            {
                //Console.WriteLine(mO["Caption"].ToString());
                Console.WriteLine("{0, -20}" + "{1, -5}", " ", mO_1["Name"].ToString());
                //Console.WriteLine("DB: Status: {0}", mO_1["DeviceName"]);
            }

            //string NamespacePath = "\\\\.\\ROOT\\StandardCimv2";
            //string ClassName = "MSFT_NetAdapter";

            ////Create ManagementClass
            //ManagementClass oClass = new ManagementClass(NamespacePath + ":" + ClassName);

            ////Get all instances of the class and enumerate them
            //foreach (ManagementObject oObject in oClass.GetInstances())
            //{
            //    //access a property of the Management object
                
            //}

            #endregion
        }
    }
}

