using System;
using System.Data;
using System.Data.SqlClient;
namespace Convert_Programs {
    
    
    public partial class ENGINEERINGDataSet {public static void IncrementOdometer(int funcCode, int uid) {
      SqlConnection conn = new SqlConnection(Properties.Settings.Default.ENGINEERINGConnectionString);
      int rowAffected = 0;
      string SQL = @"UPDATE GEN_ODOMETER SET ODO = ODO + 1 WHERE (FUNCID = @app AND USERID = @user);";
      if (conn.State == ConnectionState.Closed) {
        conn.Open();
      }
      using (SqlCommand comm = new SqlCommand(SQL, conn)) {
        comm.Parameters.AddWithValue("@app", funcCode);
        comm.Parameters.AddWithValue("@user", uid);
        try {
          rowAffected = (int)comm.ExecuteNonQuery();
        } catch (InvalidOperationException ioe) {
          throw ioe;
        }
      }

      if (rowAffected == 0) {
        SQL = @"INSERT INTO GEN_ODOMETER (ODO, FUNCID, USERID) VALUES (@odo, @app, @user);";

        using (SqlCommand comm = new SqlCommand(SQL, conn)) {
          comm.Parameters.AddWithValue("@odo", 1);
          comm.Parameters.AddWithValue("@app", (int)funcCode);
          comm.Parameters.AddWithValue("@user", uid);
          try {
            rowAffected = (int)comm.ExecuteNonQuery();
          } catch (InvalidOperationException ioe) {
            throw ioe;
          }
        }
      }
      conn.Close();
    }

    partial class GEN_ODOMETERDataTable {

    }
  }
}
