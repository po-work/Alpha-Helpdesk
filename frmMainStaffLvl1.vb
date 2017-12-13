Imports System.Security.Cryptography 'Needed for HASHING
Imports System.Text 'Needed for HASHING
Imports MySql.Data.MySqlClient
Public Class frmMainStaffLvl1
    'Defining mysql server variables
    Dim mysqlserver As String = My.Settings.mysqlip
    Dim mysqlport As String = My.Settings.mysqlport
    Dim mysqluser As String = My.Settings.mysqluser
    Dim mysqlpassword As String = My.Settings.mysqlpass
    'MySql Connection Variables
    Dim con As New MySqlConnection()
    Dim cmd As New MySqlCommand()
    Private Sub frmMainStaffLvl1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Information Variables
        Dim fname As String
        Dim sname As String
        Dim alvl As String
        'Below grabs all info about staff memeber currently logged in
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM users WHERE username = @userLogin"
            cmd.Parameters.AddWithValue("@userLogin", My.Settings.usernameforinfo)
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                'hashpass = sqlreader("hash").ToString()
                'salt = sqlreader("salt").ToString
                fname = sqlreader("fname").ToString
                sname = sqlreader("sname").ToString
                alvl = sqlreader("adminlevel").ToString
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        Finally
            con.Close()
        End Try
        'Display info about staff member
        lblStaffUser.Text = My.Settings.usernameforinfo
        lblStaffFname.Text = fname
        lblStaffSname.Text = sname
        lblStaffALvl.Text = alvl
        'Add games to combo-box
        Dim output As New List(Of String)()
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT game FROM games"
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                output.Add(sqlreader("game").ToString())
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        Finally
            con.Close()
        End Try
        'Display questions table to user
        Dim Adapter As New MySqlDataAdapter
        Dim data As New DataTable
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM questions"
            Adapter.SelectCommand = cmd
            Adapter.Fill(data)
            DGVQuestions.DataSource = data
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        'Show Customer serivce staff highest searched issue
        Dim id As Integer 'Variable to hold ID of highest issue
        Dim issue As String 'Variable to hold Highest issue
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;") 'Normal connection string
            cmd.Connection = con 'Defining connection
            cmd.CommandText = "SELECT * FROM questions WHERE timessearched = (SELECT max(timessearched) FROM questions)" 'Use of subquery in query to meet a criteria
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read() 'Reading information of issue that meets criteria
                id = sqlreader("id").ToString()
                issue = sqlreader("issuetoshow").ToString()
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        Finally
            con.Close()
        End Try
        lbHighestIssue.Items.Add("ID: " & id & " - " & "Issue: " & issue) 'Displaying issue
        'Misc executions
        Dim gamelist = output
        For Each item As String In gamelist
            cbGames.Items.Add(item)
            cbRemoveGames.Items.Add(item)
        Next
    End Sub

    Private Sub btnAddGame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddGame.Click
        btnAddGame.Enabled = False
        'Inserts new game to GAMES table
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO games(game) VALUES(@AddGame)"
            cmd.Parameters.Add("@AddGame", MySqlDbType.VarChar, 255).Value = tbGameAdd.Text
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        cmd.Parameters.Clear()
        'Refresh games
        Dim output As New List(Of String)()
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT game FROM games"
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                output.Add(sqlreader("game").ToString())
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        Finally
            con.Close()
        End Try
        'Check if currently exists in CB's, if so, then don't duplicate
        Dim gamelist = output
        Dim result As Integer = -1
        For Each item As String In gamelist
            result = cbGames.FindStringExact(item)
            If result > -1 Then

            Else
                cbGames.Items.Add(item)
            End If
            result = cbRemoveGames.FindStringExact(item)
            If result > -1 Then

            Else
                cbRemoveGames.Items.Add(item)
            End If
        Next
        'Reset TB
        tbGameAdd.Text = ""
        btnAddGame.Enabled = True
    End Sub

    Private Sub btnRemoveGame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveGame.Click
        btnRemoveGame.Enabled = False
        'Delete game from table
        Dim removegame As String = cbRemoveGames.Text
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "DELETE FROM games WHERE game = @RemoveGame"
            cmd.Parameters.Add("@RemoveGame", MySqlDbType.VarChar, 255).Value = removegame
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        cmd.Parameters.Clear()
        'Remove deleted game from CB's
        cbGames.Items.Remove(removegame)
        cbRemoveGames.Items.Remove(removegame)
        btnRemoveGame.Enabled = True
    End Sub

    Private Sub btnAddFix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddFix.Click
        btnAddFix.Enabled = False
        'Variables to store in DB
        Dim issue As String = tbIssue.Text
        Dim fix As String = tbFixForIssue.Text
        Dim game As String = cbGames.Text
        Dim issuecompare As String = issue.ToLower
        'Check if data is entered
        If game = "" Then
            MessageBox.Show("No game selected!")
            Exit Sub
        End If
        If fix = "" Then
            MessageBox.Show("No fix stated!!")
            Exit Sub
        End If
        If issue = "" Then
            MessageBox.Show("No issue stated!")
            Exit Sub
        End If
        'Insert data into QUESTIONS table
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO questions(issuetocompare,issuetoshow,game,fix,timessearched) VALUES(@issueCompare,@issueShow,@game,@fix,@times)"
            cmd.Parameters.Add("@issueCompare", MySqlDbType.VarChar, 255).Value = issuecompare
            cmd.Parameters.Add("@issueShow", MySqlDbType.VarChar, 255).Value = issue
            cmd.Parameters.Add("@game", MySqlDbType.VarChar, 255).Value = game
            cmd.Parameters.Add("@fix", MySqlDbType.VarChar, 255).Value = fix
            cmd.Parameters.Add("@times", MySqlDbType.Int64, 255).Value = 0
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        cmd.Parameters.Clear()
        'Refresh DGV for user
        Dim Adapter As New MySqlDataAdapter
        Dim data As New DataTable
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM questions"
            Adapter.SelectCommand = cmd
            Adapter.Fill(data)
            DGVQuestions.DataSource = data
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        'Misc executions
        lblSuccess.Visible = True
        btnAddFix.Enabled = False
        System.Threading.Thread.Sleep(500)
        lblSuccess.Visible = False
        btnAddFix.Enabled = True
        tbIssue.Text = ""
        tbFixForIssue.Text = ""
        btnAddFix.Enabled = True
    End Sub

    Private Sub btnRemoveFix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveFix.Click
        btnRemoveFix.Enabled = False
        'Crash/Error Prevention
        If tbInputID.Text = "" Then
            MessageBox.Show("No ID inserted!")
            Exit Sub
        End If
        'Remove stated ID
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "DELETE FROM questions WHERE ID = @id"
            cmd.Parameters.Add("@id", MySqlDbType.VarChar, 255).Value = tbInputID.Text
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        'Refresh DGV for user
        Dim Adapter As New MySqlDataAdapter
        Dim data As New DataTable
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM questions"
            Adapter.SelectCommand = cmd
            Adapter.Fill(data)
            DGVQuestions.DataSource = data
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnRemoveFix.Enabled = True
    End Sub

    Private Sub btnLogout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogout.Click
        'Allow logging out
        frmLogin.Show()
        Me.Close()
    End Sub

End Class