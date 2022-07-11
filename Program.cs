using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace Availity_CSVProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read source file, assign destination folder, both defined in the App.Config file

            string inputFile = ConfigurationSettings.AppSettings["source"].ToString();
            string destFolder = ConfigurationSettings.AppSettings["destination"].ToString();

            //Call Method to separate rows into different files
            CreateSeparateFileByHI(inputFile, destFolder);
        }

        public static void CreateSeparateFileByHI(string inputCSV, string destinationPath)
        {
            //Declare generic list of InsuranceClient class
            List<InsuranceClient> lstClients = new List<InsuranceClient>();

            //Extract elements from CSV file and add them to List
            using (var reader = new StreamReader(inputCSV))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] values = line.Split(',');
                    InsuranceClient insClient = new InsuranceClient();
                    insClient.UserId = Convert.ToInt32(values[0].ToString());
                    insClient.FName = values[1].ToString();
                    insClient.LName = values[2].ToString();
                    insClient.Version = Convert.ToInt32(values[3].ToString());
                    insClient.InsuranceCompany = values[4].ToString();
                    lstClients.Add(insClient);
                }
            }

            //Select unique insurance companies and sort them alphabetically.
            List<string> insCompanies = lstClients.Select(x => x.InsuranceCompany).Distinct().ToList();
            insCompanies.Sort();

            //Loop though each Insurance company
            foreach (string comp in insCompanies)
            {
                //List<InsuranceClient> newSet = (from x in lstClients where x.InsuranceCompany == comp select x).ToList();

                List<InsuranceClient> uniqueIds = lstClients.GroupBy(p => p.UserId).Select(g => g.Last()).ToList();
                List<InsuranceClient> uniqueVer = (from x in uniqueIds where x.InsuranceCompany == comp select x).ToList();
                List<InsuranceClient> newSet = uniqueVer.OrderBy(x => x.LName).ToList();


                string fileName = destinationPath + comp + ".CSV";

                using (var file = File.CreateText(fileName))
                {
                    foreach (InsuranceClient item in newSet)
                    {
                        StringBuilder newCSVRow = new StringBuilder();

                        newCSVRow.Append(item.UserId + "," + item.FName + "," + item.LName + "," + item.Version + "," + item.InsuranceCompany);

                        file.WriteLine(newCSVRow.ToString());
                    }
                }


            }
        }
    }

    public class InsuranceClient
    {
        private int _userid;
        private string _fname;
        private string _lname;
        private int _version;
        private string _insuranceCompany;


        public int UserId
        {
            get { return _userid; }
            set { _userid = value; }
        }
        public string FName
        {
            get { return _fname; }
            set { _fname = value; }
        }
        public string LName
        {
            get { return _lname; }
            set { _lname = value; }
        }
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }
        public string InsuranceCompany
        {
            get { return _insuranceCompany; }
            set { _insuranceCompany = value; }
        }

    }
}
