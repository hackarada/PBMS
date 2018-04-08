using System;
using System.Data;
using System.Collections.Generic;
using DAL;

namespace BLservice
{
    public class Rpt_Transaction_Detail
    {
        #region Default Constructor
        DataAccess Dal;
        public Rpt_Transaction_Detail()
        {
            Dal = new DataAccess();
        }
        #endregion

        #region Public Properties

        public int projectId { set; get; }
        public string expenditureName { set; get; }
        public string requesterFullName { set; get; }
        public string withdrawReference { set; get; }
        public string withdrawAmount { set; get; }
        public string withdrawDate { set; get; }
        public string withdrawReceiver { set; get; }

        public string projectName { set; get; }
        public string StartDate { set; get; }
        public string EndDate { set; get; }
        public string ReportGeneratedDate { set; get; }

        #endregion

        #region Methods
        public List<Rpt_Transaction_Detail> LoadAllTransactions(int projectId, string startDate, string endDate, int expndtureType)
        {
            IDataReader reader = null;
            DateConverter converterObj = new DateConverter();
            string cmdTxt = "";
            string gcStartDate = "", gcEndDate = "";
            string expndtureString = "";

            if (expndtureType != -1)
                expndtureString = " And Expenditure_Id=" + expndtureType +" ";

            if (!string.IsNullOrEmpty(startDate))
                gcStartDate = converterObj.ConvertToGregorian(startDate);
            else
                startDate = "Not Selected";




            if (!string.IsNullOrEmpty(endDate))
                gcEndDate = converterObj.ConvertToGregorian(endDate);
            else
                endDate = "Not Selected";

            if (projectId == -1 && startDate == "Not Selected" && endDate == "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Order By Withdraw_Date asc";

            else if (projectId == -1 && startDate != "Not Selected" && endDate == "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Withdraw_Date >= '" + gcStartDate + "' " + expndtureString + "Order By Withdraw_Date asc";

            else if (projectId == -1 && startDate == "Not Selected" && endDate != "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Withdraw_Date <= '" + gcEndDate + "' " + expndtureString + "Order By Withdraw_Date asc";

            else if (projectId == -1 && startDate != "Not Selected" && endDate != "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Withdraw_Date BETWEEN '" + gcStartDate + "' And '" + gcEndDate + "' " + expndtureString + "Order By Withdraw_Date asc";

            else if (projectId != -1 && startDate == "Not Selected" && endDate == "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Project_Id = " + projectId + " Order By Withdraw_Date asc";

            else if (projectId != -1 && startDate != "Not Selected" && endDate == "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Project_Id = " + projectId + " And Withdraw_Date >= '" + gcStartDate + "' " + expndtureString + "Order By Withdraw_Date asc";

            else if (projectId != -1 && startDate == "Not Selected" && endDate != "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Project_Id = " + projectId + " And Withdraw_Date <= '" + gcEndDate + "' " + expndtureString + "Order By Withdraw_Date asc";

            else if (projectId != -1 && startDate != "Not Selected" && endDate != "Not Selected")
                cmdTxt = "Select * From Rpt_Transaction_Detail Where Project_Id = " + projectId + " And Withdraw_Date BETWEEN '" + gcStartDate + "' And '" + gcEndDate + "' " + expndtureString + "Order By Withdraw_Date asc";


            List<Rpt_Transaction_Detail> detailList = new List<Rpt_Transaction_Detail>();
            try
            {
                Rpt_Transaction_Detail detailObj;
                reader = Dal.loadDataByText(cmdTxt);
                decimal withdrawAmountSum = 0;
                while (reader.Read())
                {
                    detailObj = new Rpt_Transaction_Detail();
                    ProjectBL projectObj = new ProjectBL();
                    List<ProjectBL> proList = null;
                    if (projectId != -1)
                    {
                        projectObj.Id = projectId;
                        proList = projectObj.loadByID();
                        detailObj.projectName = proList[0].Name;
                    }
                    else
                        detailObj.projectName = "All Projects";

                    detailObj.withdrawDate = converterObj.ConvertToEthiopian(reader["Withdraw_Date"].ToString());
                    detailObj.requesterFullName = reader["Management_First_Name"].ToString() + " " + reader["Management_Last_Name"].ToString();
                    detailObj.withdrawReference = reader["Withdraw_Reference_No"].ToString();
                    detailObj.withdrawAmount = decimal.Parse(reader["Withdraw_Amount"].ToString()).ToString("N");
                    detailObj.withdrawReceiver = reader["Received_By"].ToString();
                    detailObj.expenditureName = reader["Expenditure_Name"].ToString();
                    detailObj.StartDate = startDate;
                    detailObj.EndDate = endDate;
                    detailObj.ReportGeneratedDate = converterObj.ConvertToEthiopian(DateTime.Now.ToString());
                    withdrawAmountSum += decimal.Parse(detailObj.withdrawAmount);    
                    detailList.Add(detailObj);
                }
                detailObj = new Rpt_Transaction_Detail();

                detailObj.withdrawDate = "----------------------\nTotal";
                detailObj.requesterFullName = "--------------------------------\n";
                detailObj.withdrawReference = "-----------------\n";
                detailObj.withdrawAmount = "---------------------\n"+withdrawAmountSum.ToString("N");
                detailObj.withdrawReceiver = "----------------------------\n";
                detailObj.expenditureName = "-----------------------------\n";
                detailList.Add(detailObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
                Dal.closeConnection();
            }
            return detailList;
        }
        #endregion
    }
}
