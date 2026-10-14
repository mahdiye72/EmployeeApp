
using Microsoft.Data.SqlClient;
using System.Data;
string connectionString =
    "Server=.;Database=CompanyDB;Integrated Security=True;TrustServerCertificate=True;";
Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

while (true)
{
    Console.WriteLine("\n--- مدیریت کارمندان ---");
    Console.WriteLine("1. نمایش کارمندان");
    Console.WriteLine("2. ویرایش حقوق");
    Console.WriteLine("3. حذف کارمند");
    Console.WriteLine("4. افزودن کارمند جدید");
    Console.WriteLine("5. بررسی وجود کارمند");
    Console.WriteLine("6. محاسبه مجموع دو حقوق");
    Console.WriteLine("7. نمایش مجموع حقوق تمام کارمندان");

    Console.WriteLine("0. خروج");
    Console.Write("انتخاب شما: ");

    string? choice = Console.ReadLine();

    if (choice == "0")
        break;

    try
    {
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();

        if (choice == "1")
        {
              ShowEmployees(con);
        }
        else if (choice == "2")
        {
            Console.Write("شناسه کارمند: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("شناسه نامعتبر است.");
                continue;
            }

            Console.Write("حقوق جدید: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal salary))
            {
                Console.WriteLine("حقوق نامعتبر است.");
                continue;
            }

            using SqlCommand cmd =
                new SqlCommand("UpdateEmployeeSalary", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = id;

            SqlParameter p =
                cmd.Parameters.Add("@NewSalary", SqlDbType.Decimal);
            p.Precision = 18;
            p.Scale = 2;
            p.Value = salary;

            int affected = Convert.ToInt32(cmd.ExecuteScalar());

            Console.WriteLine(affected > 0
                ? "حقوق با موفقیت تغییر کرد."
                : "کارمندی با این شناسه پیدا نشد.");
        }
        else if (choice == "3")
        {
            Console.Write("شناسه کارمند برای حذف: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("شناسه نامعتبر است.");
                continue;
            }

            Console.Write("برای تأیید حذف، DELETE را وارد کن: ");
            if (Console.ReadLine() != "DELETE")
            {
                Console.WriteLine("حذف لغو شد.");
                continue;
            }

            using SqlCommand cmd =
                new SqlCommand("DeleteEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = id;

            int affected = Convert.ToInt32(cmd.ExecuteScalar());

            Console.WriteLine(affected > 0
                ? "کارمند حذف شد."
                : "کارمندی با این شناسه پیدا نشد.");
        }
        else if (choice == "4")
        {
    Console.Write("نام کارمند: ");
    /*string fullName = Console.ReadLine() ?? "";*/
    string fullName = (Console.ReadLine() ?? "").Trim();
    if (string.IsNullOrWhiteSpace(fullName))
    {
    Console.WriteLine("نام کارمند نمی‌تواند خالی باشد.");
    continue;
    }

    Console.Write("حقوق کارمند: ");
    if (!decimal.TryParse(
        Console.ReadLine(), out decimal salary))
    {
        Console.WriteLine("حقوق واردشده معتبر نیست.");
        continue;
    }
    // بررسی منفی نبودن حقوق
if (salary < 0)
{
    Console.WriteLine("حقوق نمی‌تواند منفی باشد.");
    continue;
}

    using SqlCommand cmd =
        new SqlCommand("AddEmployee", con);

    cmd.CommandType = CommandType.StoredProcedure;

    cmd.Parameters.Add(
        "@FullName", SqlDbType.NVarChar, 100
    ).Value = fullName;

    SqlParameter salaryParam =
        cmd.Parameters.Add("@Salary", SqlDbType.Decimal);

    salaryParam.Precision = 18;
    salaryParam.Scale = 2;
    salaryParam.Value = salary;

    SqlParameter idParam =
        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);

    idParam.Direction = ParameterDirection.Output;

    cmd.ExecuteNonQuery();

    Console.WriteLine(
        $"کارمند با شناسه {idParam.Value} اضافه شد.");
        }
        
else if (choice == "5")
{
    Console.Write("شناسه کارمند: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("شناسه نامعتبر است.");
        continue;
    }

    using SqlCommand cmd =
        new SqlCommand("CheckEmployeeExists", con);

    cmd.CommandType = CommandType.StoredProcedure;

    cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = id;

    bool exists = Convert.ToBoolean(cmd.ExecuteScalar());

    if (exists)
    {
        Console.WriteLine("کارمند وجود دارد.");
    }
    else
    {
        Console.WriteLine("کارمند وجود ندارد.");
    }
}
else if (choice == "6")
{
    Console.Write("حقوق اول: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal salary1))
    {
        Console.WriteLine("حقوق نامعتبر است.");
        continue;
    }

    Console.Write("حقوق دوم: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal salary2))
    {
        Console.WriteLine("حقوق نامعتبر است.");
        continue;
    }

    decimal total = CalculateTotalSalary(salary1, salary2);
    Console.WriteLine($"مجموع حقوق: {total}");
}
else if (choice == "7")
{
    using SqlCommand cmd = new SqlCommand("GetTotalSalary", con);
    cmd.CommandType = CommandType.StoredProcedure;

    object? result = cmd.ExecuteScalar();

    decimal totalSalary = result == DBNull.Value
        ? 0
        : Convert.ToDecimal(result);

    Console.WriteLine($"مجموع حقوق تمام کارمندان: {totalSalary:N0}");
}

        else
        {
            Console.WriteLine("گزینه نامعتبر است.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("خطا: " + ex.Message);
    }
    static void ShowEmployees(SqlConnection con)
{
    using SqlCommand cmd =
        new SqlCommand("GetEmployees", con);

    cmd.CommandType = CommandType.StoredProcedure;

    using SqlDataReader reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine(
            $"ID: {reader["EmployeeID"]} | " +
            $"Name: {reader["FullName"]} | " +
            $"Salary: {reader["Salary"]}");
    }
}
}
static decimal CalculateTotalSalary(decimal salary1, decimal salary2)
{
    return salary1 + salary2;
}
// Employee management application
// EmployeeApp - Employee Management System