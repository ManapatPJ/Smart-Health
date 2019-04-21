Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class AppInterface

    Dim rec_table As New DataTable
    Dim isRequested As Boolean
    Dim dateGetRequested As String

    Dim response_table As New DataTable


    Sub initialize()


        response_table.Columns.Add("ID")
        response_table.Columns.Add("Response")
        response_table.Columns.Add("Name")




        lbl_ResponseTime.Text = "Emergency Response as of: " & DateAndTime.Now.ToString("yyyy-MM-dd HH:mm")
        rec_table.Columns.Add("Date")
        rec_table.Columns.Add("First Name")
        rec_table.Columns.Add("Last Name")
        rec_table.Columns.Add("Temperature")
        rec_table.Columns.Add("Pulse")
        rec_table.Columns.Add("Blood Pressure")
        rec_table.Columns.Add("Respiration")
        rec_table.Columns.Add("Latitude/Longitude")
        rec_table.Columns.Add("Requested")
        rec_table.Columns.Add("Distance", GetType(Double))


    End Sub
    Private Sub AppInterface_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        initialize()

        bgw_startup.RunWorkerAsync()

        bgw_startup.WorkerReportsProgress = True

    End Sub

    Private Sub bgw_startup_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgw_startup.DoWork
        Try
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            request = DirectCast(WebRequest.Create("https://api.thingspeak.com/channels/733323/feeds.json?api_key=A7U345D8V1DG1MAM&results=100"), HttpWebRequest)
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            Dim rawresp As String
            rawresp = reader.ReadToEnd()

            Dim json As JObject
            json = JObject.Parse(rawresp)

            Dim txt As String()
            txt = Split(json("feeds").ToString, "{")

            Dim txt2 As String()
            For i As Integer = 1 To UBound(txt)
                txt2 = Split(txt(i), "}")




                Dim data As String
                data = txt2(0)

                Dim dateArray As String()

                Dim dateTime As String = ""
                dateArray = Split(data, "created_at")
                dateTime = dateArray(1).Substring(4, 20).Replace("T", " ").Replace("Z", " ")

                Dim final As String()
                'MsgBox(txt2(0).Replace("field", "").Replace("" & ControlChars.Quote & "", "").Replace(" ", ""))
                final = Split(txt2(0).Replace("field", "").Replace("" & ControlChars.Quote & "", "").Replace(" ", ""), vbNewLine)

                Dim latlong As String()
                latlong = Split(final(9).Replace(",", "").Remove(0, 2), "/")



                rec_table.Rows.Add(dateTime,
                                   final(3).Replace(",", "").Remove(0, 2),
                                   final(4).Replace(",", "").Remove(0, 2),
                                   final(5).Replace(",", "").Remove(0, 2),
                                   final(6).Replace(",", "").Remove(0, 2),
                                   final(7).Replace(",", "").Remove(0, 2),
                                   final(8).Replace(",", "").Remove(0, 2),
                                   Math.Round(CDbl(latlong(0)), 4) & "/" & Math.Round(CDbl(latlong(1)), 4),
                                   final(10).Replace(",", "").Remove(0, 2),
                                   Math.Round(distance(14.187671, 121.125084, latlong(0), latlong(1), "K"), 2))
                'rec_table.AcceptChanges()

                'TextBox1.Text += "Entry No.:" & i & " " & final(3).Remove(0, 3) + final(4).Remove(0, 3) + final(5).Remove(0, 3) + final(6).Remove(0, 3) + final(7).Remove(0, 3) + final(8).Remove(0, 3) + vbNewLine
                'TextBox1.Text = TextBox1.Text.Replace("" & ControlChars.Quote & "", "")
            Next


        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub bgw_startup_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgw_startup.RunWorkerCompleted

        DataGridView1.DataSource = rec_table
        DataGridView1.Update()
        DataGridView1.Sort(DataGridView1.Columns(0), System.ComponentModel.ListSortDirection.Descending)
        DataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        DataGridView1.Columns(8).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        tbl_loading.Visible = False
    End Sub
    Private Sub bgw_checker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgw_checker.DoWork
        Try
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader

            request = DirectCast(WebRequest.Create("https://api.thingspeak.com/channels/733323/feeds.json?api_key=A7U345D8V1DG1MAM&results=100"), HttpWebRequest)

            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())

            Dim rawresp As String
            rawresp = reader.ReadToEnd()

            Dim json As JObject
            json = JObject.Parse(rawresp)

            Dim txt As String()
            txt = Split(json("feeds").ToString, "{")

            Dim txt2 As String()
            For i As Integer = 1 To UBound(txt)
                txt2 = Split(txt(i), "}")




                Dim data As String
                data = txt2(0)

                Dim dateArray As String()

                Dim dateTime As String = ""
                dateArray = Split(data, "created_at")
                dateTime = dateArray(1).Substring(4, 20).Replace("T", " ").Replace("Z", " ")


                Dim foundDate As DataRow() = rec_table.[Select]("Date = '" & dateTime & "'")

                If foundDate.Length <> 0 Then
                    GoTo nextt
                End If

                bgw_checker.ReportProgress(10)

                Dim final As String()
                'MsgBox(txt2(0).Replace("field", "").Replace("" & ControlChars.Quote & "", "").Replace(" ", ""))
                final = Split(txt2(0).Replace("field", "").Replace("" & ControlChars.Quote & "", "").Replace(" ", ""), vbNewLine)


                Dim latlong As String()
                latlong = Split(final(9).Replace(",", "").Remove(0, 2), "/")



                rec_table.Rows.Add(dateTime,
                                   final(3).Replace(",", "").Remove(0, 2),
                                   final(4).Replace(",", "").Remove(0, 2),
                                   final(5).Replace(",", "").Remove(0, 2),
                                   final(6).Replace(",", "").Remove(0, 2),
                                   final(7).Replace(",", "").Remove(0, 2),
                                   final(8).Replace(",", "").Remove(0, 2),
                                   Math.Round(CDbl(latlong(0)), 4) & "/" & Math.Round(CDbl(latlong(1)), 4),
                                   final(10).Replace(",", "").Remove(0, 2),
                                   Math.Round(distance(14.187671, 121.125084, latlong(0), latlong(1), "K"), 2))
                rec_table.AcceptChanges()
                Me.Invoke(Sub()
                              Try
                                  DataGridView1.DataSource = Nothing
                                  DataGridView1.Update()
                                  DataGridView1.Refresh()

                                  DataGridView1.DataSource = rec_table
                                  DataGridView1.Update()
                                  DataGridView1.Refresh()
                                  Label2.Text = Label2.Text + 1
                                  DataGridView1.Sort(DataGridView1.Columns(0), System.ComponentModel.ListSortDirection.Descending)
                                  DataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                                  DataGridView1.Columns(8).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

                              Catch ex As Exception

                              Finally
                                  If final(10).Replace(",", "").Remove(0, 2) = lbl_RequestCode.Text Then
                                      isRequested = True
                                      dateGetRequested = dateTime.ToString
                                  End If
                                  If isRequested = True Then
                                      'Alarm here and change color of row 
                                      bgw_alarm.RunWorkerAsync()
                                      Dim responseRequest As DialogResult
                                      responseRequest = MessageBox.Show("You are requested by a patient. Accept his/her admission?", "EMERGENCY!", MessageBoxButtons.YesNo)
                                      'Enable alarm
                                      If responseRequest = vbNo Then
                                          response_table.Rows.Add(DataGridView1.Rows.Count, "No", final(4).Replace(",", "").Remove(0, 2) + " " + final(3).Replace(",", "").Remove(0, 2))
                                          sendResponse("No")
                                      Else
                                          response_table.Rows.Add(DataGridView1.Rows.Count, "Yes", final(4).Replace(",", "").Remove(0, 2) + " " + final(3).Replace(",", "").Remove(0, 2))
                                          sendResponse("Yes")
                                      End If
                                      My.Computer.Audio.Stop()
                                      bgw_alarm.CancelAsync()

                                      DataGridView2.DataSource = Nothing
                                      DataGridView2.Update()
                                      DataGridView2.Refresh()

                                      DataGridView2.DataSource = response_table
                                      DataGridView2.Update()
                                      DataGridView2.Refresh()
                                      'Reinitialize for next request
                                      isRequested = False
                                      dateGetRequested = ""
                                  End If
                              End Try
                          End Sub)


                'TextBox1.Text += "Entry No.:" & i & " " & final(3).Remove(0, 3) + final(4).Remove(0, 3) + final(5).Remove(0, 3) + final(6).Remove(0, 3) + final(7).Remove(0, 3) + final(8).Remove(0, 3) + vbNewLine
                'TextBox1.Text = TextBox1.Text.Replace("" & ControlChars.Quote & "", "")
nextt:

            Next

            bgw_checker.ReportProgress(10)
        Catch ex As Exception
            ' MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.BackColor = Color.Green
        bgw_checker.RunWorkerAsync()
    End Sub

    Private Sub bgw_checker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgw_checker.RunWorkerCompleted
        DataGridView1.DataSource = rec_table

        
        DataGridView1.Update()
        'Label1.Text = Label1.Text + 1
        bgw_checker.RunWorkerAsync()
    End Sub

    Private Sub bgw_checker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
        'dg_records.DataSource = Nothing
        'dg_records.DataSource = rec_table
        'dg_records.Update()

        'DataGridView1.DataSource = Nothing
        'DataGridView1.DataSource = rec_table

        'DataGridView1.Update()
        'DataGridView1.Refresh()
        Label2.Text = Label2.Text + 1
        DataGridView1.Sort(DataGridView1.Columns(0), System.ComponentModel.ListSortDirection.Descending)
    End Sub




    Public Function distance(ByVal lat1 As Double, ByVal lon1 As Double, ByVal lat2 As Double, ByVal lon2 As Double, ByVal unit As Char) As Double
        If lat1 = lat2 And lon1 = lon2 Then
            Return 0
        Else
            Dim theta As Double = lon1 - lon2
            Dim dist As Double = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta))
            dist = Math.Acos(dist)
            dist = rad2deg(dist)
            dist = dist * 60 * 1.1515
            If unit = "K" Then
                dist = dist * 1.609344
            ElseIf unit = "N" Then
                dist = dist * 0.8684
            End If
            Return dist
        End If
    End Function

    Private Function deg2rad(ByVal deg As Double) As Double
        Return (deg * Math.PI / 180.0)
    End Function

    Private Function rad2deg(ByVal rad As Double) As Double
        Return rad / Math.PI * 180.0
    End Function





    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles tbl_loading.Paint

    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub DataGridView1_CellContentDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentDoubleClick


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

    End Sub

    Private Sub bgw_alarm_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgw_alarm.DoWork
        My.Computer.Audio.Play(Environment.CurrentDirectory & "\Alarm\emergency_alarm_tone.wav", AudioPlayMode.Background)
    End Sub


    Sub sendResponse(reply As String)
        Try
retryy:
            Dim url As String = "https://api.thingspeak.com/update?api_key=WZ4E8T4CISW3CBOR&field1=" & DataGridView1.Rows.Count & "&field2=" & reply & ""
            ' Using WebRequest
            Dim request As WebRequest = WebRequest.Create(url)
            Dim response As WebResponse = request.GetResponse()
            Dim result As String = New StreamReader(response.GetResponseStream()).ReadToEnd()
            ' Using WebClient
            Dim result1 As String = New WebClient().DownloadString(url)

            If result = "0" Then
                GoTo retryy
            Else
                MsgBox("Sending Successful.")
            End If

        Catch ex As Exception
            MsgBox("Sending Failed. " & ex.ToString)
        End Try
    End Sub
End Class