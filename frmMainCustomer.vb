Imports MySql.Data.MySqlClient
Public Class frmMainCustomer
    'MySql Variables
    Dim mysqlserver As String = My.Settings.mysqlip
    Dim mysqlport As String = My.Settings.mysqlport
    Dim mysqluser As String = My.Settings.mysqluser
    Dim mysqlpassword As String = My.Settings.mysqlpass
    Dim con As New MySqlConnection()
    Dim cmd As New MySqlCommand()
    Dim ids As New List(Of Integer)()
    Private Sub btnTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        frmLogin.Show()
        Me.Close()
    End Sub

    Private Sub frmMainCustomer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Information Variables
        Dim fname As String
        Dim sname As String
        'Below grabs all info about memeber currently logged in
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM users WHERE username = @userLogin"
            cmd.Parameters.AddWithValue("@userLogin", My.Settings.usernameforinfo)
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                fname = sqlreader("fname").ToString
                sname = sqlreader("sname").ToString
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            End
        Finally
            con.Close()
        End Try
        'Show information about user
        lblStaffUser.Text = My.Settings.usernameforinfo
        lblStaffFname.Text = fname
        lblStaffSname.Text = sname
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
            End
        Finally
            con.Close()
        End Try

        Dim gamelist = output
        For Each item As String In gamelist
            cbGames.Items.Add(item)
        Next
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        'RESET IN HERE
        ListBox1.Items.Clear()
        tbUserIssue.Text = ""
        lblIssueGame.Visible = False
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        btnSearch.Enabled = False
        ListBox1.Items.Clear()
        'Error Prevention
        If cbGames.Text = "" Then
            MessageBox.Show("No game selected!")
        End If
        If tbUserIssue.Text = "" Then
            MessageBox.Show("No issue stated!")
        End If
        'Setting of labels
        lblIssueGame.Text = cbGames.Text
        lblIssueGame.Visible = True
        'Variable Definitions
        Dim tbissue As String = tbUserIssue.Text.ToLower
        Dim counter1 As Integer = 0
        Dim counter2 As Integer = 0
        Dim issuecompare As String
        Dim issueshow As String
        Dim fixshow As String
        Dim timessearched As Integer
        'Information from DB Variables
        'split user input into array
        Dim userinputarray() As String = tbUserIssue.Text.Split(" ")
        'Find issues that corrolate with user issue
        For Each item As String In ids
            Try
                cmd.CommandText = "SELECT * FROM questions WHERE id = " & item
                con.Open()
                cmd.ExecuteNonQuery()
                Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
                While sqlreader.Read
                    issuecompare = sqlreader("issuetocompare").ToString()
                    issueshow = sqlreader("issuetoshow").ToString()
                    fixshow = sqlreader("fix").ToString()
                    timessearched = sqlreader("timessearched").ToString() 'Grab timessearched currently stored for requested issue
                End While
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
                End
            Finally
                con.Close()
            End Try
            For counter1 = 0 To UBound(userinputarray, 1)
                If issuecompare.ToLower.Contains(userinputarray(counter1).ToLower) Then
                    ListBox1.Items.Add("Possible Error(s): " & issueshow & " - " & "Fix: " & fixshow)
                    timessearched = timessearched + 1 'Increase timessearched by 1
                    Try
                        cmd.CommandText = "UPDATE questions SET timessearched = @newTimesSearched WHERE id = @newID" 'Update timessearched for that specific issue
                        cmd.Parameters.Add("@newTimesSearched", MySqlDbType.Int64, 255).Value = timessearched
                        cmd.Parameters.Add("@newID", MySqlDbType.Int64, 255).Value = item
                        con.Open()
                        cmd.ExecuteNonQuery()
                    Catch ex As Exception
                        MessageBox.Show("Error: " & ex.Message)
                        End
                    Finally
                        con.Close()
                    End Try
                    cmd.Parameters.Clear()
                    Exit For
                End If
            Next
        Next
        btnSearch.Enabled = True
    End Sub

    Private Sub btnLogout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogout.Click
        frmLogin.Show()
        Me.Close()
    End Sub

    Private Sub cbGames_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbGames.SelectedIndexChanged
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM questions WHERE game = @game"
            'If cbGames.Text = cmd.Parameters.AddWithValue("@game", cbGames.Text).ToString() Then
            '    cmd.Parameters.RemoveAt("@game")
            'End If
            cmd.Parameters.AddWithValue("@game", cbGames.Text)
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                'issuetocompare.Add(sqlreader("issuetocompare").ToString())
                ids.Add(sqlreader("ID").ToString())
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            End
        Finally
            con.Close()
        End Try
    End Sub
End Class