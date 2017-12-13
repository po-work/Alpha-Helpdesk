Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography 'Needed for HASHING
Imports System.Text 'Needed for HASHING
Namespace My
    Partial Friend Class MyApplication
        Public Function CreateAdminLevelCode()
            Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider
            Dim buff(15) As Byte
            rng.GetBytes(buff)
            Return Convert.ToBase64String(buff)
        End Function
        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            Dim mysqlserver As String = My.Settings.mysqlip
            Dim mysqlport As Integer = My.Settings.mysqlport
            Dim mysqluser As String = My.Settings.mysqluser
            Dim mysqlpassword As String = My.Settings.mysqlpass
            Dim serveradmincode As String
            Dim userstablecontents As String
            Dim con As New MySqlConnection()
            Dim cmd As New MySqlCommand()
            'CREATES DB & USERS TABLE & ADMINCODE TABLE & SET VARB TO SEE IF ADMINCODE HAS ANYTHING IN IT
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=mysql;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS main"
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
            'Create Users Table
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS users(ID int NOT NULL AUTO_INCREMENT, username varchar(255),hash varchar(255),salt varchar(255),fname char(255),sname char(255),adminlevel int, PRIMARY KEY(id))"
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
            'Create Admincode Table
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS admincode(admincode varchar(255))"
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
            'Pull any data in AdminCode table
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "SELECT * FROM admincode"
                cmd.ExecuteNonQuery()
                Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
                While sqlreader.Read()
                    serveradmincode = sqlreader("admincode").ToString()
                End While
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
            'IF CONTAINS NOTHING THEN CREATE DATA TO GO INTO IT
            If serveradmincode = "" Then
                Try 'POPULATE ADMIN CODE TABLE
                    con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                    con.Open()
                    cmd.Connection = con
                    cmd.CommandText = "INSERT INTO admincode(admincode) VALUES(@code)"
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar, 255).Value = CreateAdminLevelCode()
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message)
                Finally
                    con.Close()
                End Try
            End If
            'CHECK IF USERS TABLE IS CURRENTLY POPULATED
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                cmd.Connection = con
                cmd.CommandText = "SELECT * FROM users"
                'cmd.Parameters.AddWithValue("@number", 1)
                con.Open()
                cmd.ExecuteNonQuery()
                Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
                While sqlreader.Read()
                    userstablecontents = sqlreader("username").ToString()
                End While
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
                Exit Sub
            Finally
                con.Close()
            End Try
            'POPULATE USERS TABLE WITH MASTER ADMIN LOGIN // USER = MainAdmin123 - PASS = Defaultpassword
            If userstablecontents = "" Then
                Try
                    con.Open()
                    cmd.Connection = con
                    cmd.CommandText = "INSERT INTO users(username, hash, salt, fname, sname, adminlevel) VALUES(@user, @hash, @salt, @fname, @sname, @alvl)"
                    cmd.Parameters.Add("@user", MySqlDbType.VarChar, 255).Value = "MainAdmin123"
                    cmd.Parameters.Add("@hash", MySqlDbType.VarChar, 255).Value = "tGp4rfVxe1jwQjpd7yx8GwiMVqsP88n9Vcs93x/D3WU="
                    cmd.Parameters.Add("@salt", MySqlDbType.VarChar, 255).Value = "+TY2ktlNH5LcgT0m5rvyEw=="
                    cmd.Parameters.Add("@fname", MySqlDbType.String, 255).Value = "Main"
                    cmd.Parameters.Add("@sname", MySqlDbType.String, 255).Value = "Admin"
                    cmd.Parameters.Add("@alvl", MySqlDbType.Int16, 255).Value = 2
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message)
                Finally
                    con.Close()
                End Try
            End If
            'CREATE QUESTIONS TABLE
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS questions(ID int NOT NULL AUTO_INCREMENT,issuetocompare varchar(255),issuetoshow varchar(255),game varchar(255),fix varchar(255),timessearched int,PRIMARY KEY(id))"
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
            'CREATE GAMES TABLE
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS games(game varchar(255))"
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
        End Sub
    End Class
End Namespace

