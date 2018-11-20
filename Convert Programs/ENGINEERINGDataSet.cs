using System;
using System.Data;
using System.Data.SqlClient;
namespace Convert_Programs {


	public partial class ENGINEERINGDataSet {

		/// <summary>
		/// Dump an error into the db.
		/// </summary>
		/// <param name="e">An <see cref="Exception"/> object.</param>
		public static void ProcessError(Exception e) {
			int uid_ = 0;
			using (ENGINEERINGDataSetTableAdapters.QueriesTableAdapter q_ =
				new ENGINEERINGDataSetTableAdapters.QueriesTableAdapter()) {
				uid_ = Convert.ToInt32(q_.UserQuery(System.Environment.UserName));
				string[] stack = e.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				string offender = stack[stack.Length - 1].Trim();
				int len = offender.Length > 255 ? 255 : offender.Length;
				string msg_ = string.Format(@"Error `{0}' occurred {1}.", e.Message, offender);
				int aff = q_.InsertError(
					DateTime.Now,
					uid_,
					e.HResult,
					e.Message,
					offender.Substring(0, len),
					false,
					@"CONVERT PROGRAMS");
				if (aff > 0) {
					msg_ += "\n" + @"This error has been reported.";
					System.Windows.Forms.MessageBox.Show(msg_, @"Redbrick Error",
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Error);
				} else {
					msg_ += "\n" + @"This error failed to be reported.";
					System.Windows.Forms.MessageBox.Show(msg_, @"Redbrick Error",
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Error);
				}
			}
		}

		public static void IncrementOdometer(int funcCode, int uid) {
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

		public static void IncrementOdometerByAmount(int funcCode, int uid, int amount) {
			SqlConnection conn = new SqlConnection(Properties.Settings.Default.ENGINEERINGConnectionString);
			int rowAffected = 0;
			string SQL = @"UPDATE GEN_ODOMETER SET ODO = ODO + @amount WHERE (FUNCID = @app AND USERID = @user);";
			if (conn.State == ConnectionState.Closed) {
				conn.Open();
			}
			using (SqlCommand comm = new SqlCommand(SQL, conn)) {
				comm.Parameters.AddWithValue("@app", funcCode);
				comm.Parameters.AddWithValue("@user", uid);
				comm.Parameters.AddWithValue("@amount", amount);
				try {
					rowAffected = (int)comm.ExecuteNonQuery();
				} catch (InvalidOperationException ioe) {
					throw ioe;
				}
			}

			if (rowAffected == 0) {
				SQL = @"INSERT INTO GEN_ODOMETER (ODO, FUNCID, USERID) VALUES (@odo, @app, @user);";

				using (SqlCommand comm = new SqlCommand(SQL, conn)) {
					comm.Parameters.AddWithValue("@odo", amount);
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
	}
}
