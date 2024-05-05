Imports System
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections.Specialized
Imports System.Net
Imports GsmComm.GsmCommunication
Imports GsmComm.PduConverter
Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FormLoadCode()
    End Sub

#Region "Free SMS"
    Private Sub SendSMSButton_Click(sender As Object, e As EventArgs) Handles SendSMSButton.Click
        Dim num = PhoneNumber.Text
        Dim sms = SmsMasseg.Text
        Dim client As WebClient = New WebClient()
        Dim res As Byte() = client.UploadValues("http://textbelt.com/text", New NameValueCollection() From {
            {"phone", num},
            {"message", sms},
            {"key", "textbelt"}
        })
        Dim resString = Encoding.UTF8.GetString(res)
        'MessageBox.Show(resString, "Response");
        If resString.Contains("true") = True Then
            MessageBox.Show("SMS Sent Successfuly", "Sent!")
        ElseIf resString.Contains("Only one test text message is allowed per day.") = True Then
            MessageBox.Show("1 SMS Per Day!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf resString.Contains("disabled") = True Then
            MessageBox.Show("Service is down", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf resString.Contains("Missing") = True Then
            MessageBox.Show("Invalid Phone Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf resString.Contains("Abusive messages") = True Then
            MessageBox.Show("Abusive Message", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
#End Region
#Region "Send SMS Via GSM Modem"
    Private _GsmCom As GsmCommMain
    Public Sub FormLoadCode()
        'Assign Ports
        ComPort.Items.Add("COM1")
        ComPort.Items.Add("COM2")
        ComPort.Items.Add("COM3")
        ComPort.Items.Add("COM4")
        ComPort.Items.Add("COM5")
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        If Equals(ComPort.Text, "") Then
            MessageBox.Show("Invalid Port Name")
            Return
        End If
        _GsmCom = New GsmCommMain(ComPort.Text, 9600, 150)
        Cursor.Current = Cursors.Default
        Dim retry As Boolean
        Do
            retry = False
            ' trying to connect
            Try
                Cursor.Current = Cursors.WaitCursor
                _GsmCom.Open()


                MessageBox.Show("Modem Connected Sucessfully..!")
                btnConnect.Enabled = False
                lblStatus.Text = "Modem is connected..!"
            Catch __unusedException1__ As Exception
                Cursor.Current = Cursors.Default
                If MessageBox.Show(Me, "GSM Modem is not available", "Check again", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) = DialogResult.Retry Then
                    retry = True
                Else
                    Return
                End If
            End Try
        Loop While retry
    End Sub

    Private Sub NyX_Button1_Click(sender As Object, e As EventArgs) Handles NyX_Button1.Click
        Try
            ' Send SMS
            Cursor.Current = Cursors.WaitCursor
            Dim _PDU As SmsSubmitPdu
            Cursor.Current = Cursors.Default
            _PDU = New SmsSubmitPdu(txtSMS.Text, txtNumber.Text)
            Dim times = 1
            For i = 0 To times - 1
                _GsmCom.SendMessage(_PDU)
            Next

            MessageBox.Show("Message Sent Succesfully..!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            Call MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub NyX_ControlBox1_Click(sender As Object, e As EventArgs) Handles NyX_ControlBox1.Click
        'Call Application.Exit()
    End Sub
#End Region

End Class
