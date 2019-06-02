﻿Imports System.IO
Imports System.Math
Imports System.Globalization
Imports System.Threading
Imports System.Text
Imports System.Management

'------- Korrel groepen in de inlaat stroom------
Public Structure Korrel_struct
    Public dia_small As Double          'Particle diameter [mu]
    Public dia_big As Double            'Particle diameter [mu]
    Public dia_ave As Double            'Particle diameter [mu]
    Public class_wght_cum_pro As Double 'group_weight_cum in de inlaat stroom [% weight]
    Public class_wght_pro As Double     'group_weight in de inlaat stroom [% weight]
    Public class_wght_kg As Double      'group_weight in de inlaat stroom [kg]
    Public verlies As Double            'verlies (niet gevangen) [-]
End Structure

Public Class Form1
    Public korrel_grp(22) As Korrel_struct    '22 korrel groepen

    'Type AC;Inlaatbreedte;Inlaathoogte;Inlaatlengte;Inlaat hartmaat;Inlaat afschuining;
    'Uitlaat keeldia inw.;Uitlaat flensdiameter inw.;Lengte insteekpijp inw.;
    'Lengte romp + conus;Lengte romp;Lengte conus;Dia_conus / 3P-pijp;Lengte 3P-pijp;Lengte 3P-conus;Kleine dia 3P-conus",

    Public Shared cyl_dimensions() As String = {
    "AC-300;0.34;0.77;0.6;0.63;0.3;0.68;0.68;0.892;3.36;1.312;2.048;0.4;0.6;0.6;0.25",
    "AC-350;0.32;0.7;0.6;0.617;0.3;0.63;0.63;0.892;3.36;1.312;2.048;0.4;0.6;0.6;0.25",
    "AC-435;0.282;0.64;0.6;0.6;0.3;0.56;0.56;0.892;3.36;1.312;2.048;0.4;0.6;0.6;0.25",
    "AC-550;0.25;0.57;0.6;0.58;0.3;0.45;0.56;0.892;3.36;1.312;2.048;0.4;0.6;0.6;0.25",
    "AC-750;0.216;0.486;0.6;0.57;0.3;0.365;0.56;0.892;3.36;1.312;2.048;0.4;0.6;0.6;0.25",
    "AC-850;0.203;0.457;0.6;0.564;0.3;0.307;0.428;0.892;3.797;1.312;2.485;0.4;0.6;0.6;0.25",
    "AC-1850;0.136;0.31;0.6;0.53;0.3;0.15;0.25;0.892;3.797;1.312;2.485;0.4;0.6;0.6;0.25"}

    'FRACTIONELE VERLIESCIJFERS volgens norm 503
    'Verlies aan de hand van de deeltjes grootte
    'min dia[mm];max dia[mm],AC300;AC350;AC435;AC550;AC750;AC850;AC850;AC1850;AC1850
    Public Shared frac_verlies() As String = {
     "0;2;97.00;95.00;87.00;80.00;75.00;70.00;60.00;60.00;30.00",
    "2;4;76.00;70.00;60.00;47.00;40.00;30.00;26.00;20.00;7.00",
    "4;6;54.00;48.00;40.00;30.00;25.00;16.00;9.00;12.00;3.00",
    "6;8;45.00;32.00;21.00;17.00;14.00;8.70;3.70;6.50;1.50",
    "8;10;36.00;22.00;12.00;10.00;8.00;5.15;1.18;4.00;1.00",
    "10;12;29.00;16.00;8.00;6.50;4.60;3.40;1.10;2.50;0.60",
    "12;14;20.50;11.00;5.50;3.50;2.70;2.40;0.65;1.70;0.45",
    "14;16;14.00;7.50;3.00;2.20;1.60;1.60;0.50;1.20;0.35",
    "16;18;11.00;5.50;2.20;1.40;1.10;1.10;0.35;0.85;0.25",
    "18;20;8.40;4.00;1.60;0.90;0.75;0.75;0.25;0.60;0.20",
    "20;25;5.50;2.50;1.00;0.45;0.40;0.40;0.16;0.40;0.15",
    "25;30;4.20;1.60;0.50;0.18;0.15;0.15;0.10;0.15;0.08",
    "30;100;3.20;0.80;0.15;0.07;0.06;0.05;0.045;0.05;0.04"}

    'FRACTIONELE VERLIESCIJFERS volgens norm 503 
    '[mm]; <  2; 2 - 4; 4 - 6; 6 - 8; 8 - 10; 10 - 12; 12 - 14; 14 - 16; 16 - 18; 18 - 20; 20 - 25; 25 - 30; > 30
    Public Shared frac_verlies2() As String = {
    "AC300;97.00;76.00;54.00;45.00;36.00;29.00;20.50;14.00;11.00;8.40;5.50;4.20;3.20",
    "AC350;95.00;70.00;48.00;32.00;22.00;16.00;11.00;7.50;5.50;4.00;2.50;1.60;0.80",
    "AC435;87.00;60.00;40.00;21.00;12.00;8.00;5.50;3.00;2.20;1.60;1.00;0.50;0.15",
    "AC550;80.00;47.00;30.00;17.00;10.00;6.50;3.50;2.20;1.40;0.90;0.45;0.18;0.07",
    "AC750;75.00;40.00;25.00;14.00;8.00;4.60;2.70;1.60;1.10;0.75;0.40;0.15;0.06",
    "AC850;70.00;30.00;16.00;8.70;5.15;3.40;2.40;1.60;1.10;0.75;0.40;0.15;0.05",
    "AC850;60.00;26.00;9.00;3.70;1.18;1.10;0.65;0.50;0.35;0.25;0.16;0.10;0.045",
    "AC1850;60.00;20.00;12.00;6.50;4.00;2.50;1.70;1.20;0.85;0.60;0.40;0.15;0.05",
    "AC1850;30.00;7.00;3.00;1.50;1.00;0.60;0.45;0.35;0.25;0.20;0.15;0.08;0.04"}


    'Nieuwe reken methode, verdeling volgens Weibull verdeling
    'm1,k1,a1 als d < d_krit
    'm2,k2,a2 als d > d_krit
    ' type;dkrit;m1;k1;a1;m2;k2;a2
    Public Shared rekenlijnen() As String = {
    "AC300;     12.2;   1.15;   7.457;  1.005;      8.5308;     1.6102; 0.4789",
    "AC350;     10.2;   1.0;    5.3515; 1.0474;     4.4862;     2.4257; 0.6472",
    "AC435;     8.93;   0.69;   4.344;  1.139;      4.2902;     1.3452; 0.5890",
    "AC550;     8.62;   0.527;  3.4708; 0.9163;     3.3211;     1.7857; 0.7104",
    "AC750;     8.3;    0.50;   2.8803; 0.8355;     4.0940;     1.0519; 0.6010",
    "AC850;     7.8;    0.52;   1.9418; 0.73705;    -0.1060;    2.0197; 0.7077",
    "AC850+afz; 10;     0.5187; 1.6412; 0.8386;     4.2781;     0.06777;0.3315",
    "AC1850;    9.3;    0.50;   1.1927; 0.5983;     -0.196;     1.3687; 0.6173",
    "AC1850+afz;10.45;  0.4617; 0.2921; 0.4560;     -0.2396;    0.1269; 0.3633"}

    Public weerstand_coef(7) As Double               'Poly Coefficients, Polynomial regression

    '----------- directory's-----------
    Dim dirpath_Eng As String = "N:\Engineering\VBasic\Cyclone_sizing_input\"
    Dim dirpath_Rap As String = "N:\Engineering\VBasic\Cyclone_rapport_copy\"
    Dim dirpath_tmp As String = "C:\Tmp\"
    Dim ProcID As Integer = Process.GetCurrentProcess.Id
    Dim dirpath_Temp As String = "C:\Temp\" & ProcID.ToString


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim hh, life_time, i As Integer
        Dim words() As String
        Dim separators() As String = {";"}
        Dim Pro_user, HD_number As String
        Dim nu, nu2 As Date
        Dim user_list As New List(Of String)
        Dim hard_disk_list As New List(Of String)
        Dim pass_name As Boolean = False
        Dim pass_disc As Boolean = False

        '------ allowed users with hard disc id's -----
        user_list.Add("user")
        hard_disk_list.Add("058F63646471")      'Privee PC, graslaan25

        user_list.Add("GerritP")
        hard_disk_list.Add("S2R6NX0H740154H")  'VTK PC, GP

        user_list.Add("GerritP")
        hard_disk_list.Add("0008_0D02_003E_0FBB.")       'VTK laptop, GP

        user_list.Add("FredKo")
        hard_disk_list.Add("JR10006P02Y6EE")    'VTK laptop, FKo

        user_list.Add("VittorioS")
        hard_disk_list.Add("002427108605")      'VTK laptop, Vittorio

        user_list.Add("keess")
        hard_disk_list.Add("002410146654")      'VTK laptop, KeesS

        user_list.Add("JanK")
        hard_disk_list.Add("0025_38B4_71B4_88FC.") 'VTK laptop, Jank

        user_list.Add("JeroenA")
        hard_disk_list.Add("171095402070")       'VTK desktop, Jeroen

        user_list.Add("JeroenA")
        hard_disk_list.Add("170228801578")       'VTK laptop, Jeroen disk 1
        hard_disk_list.Add("MCDBM1M4F3QRBEH6")   'VTK laptop, Jeroen disk 2
        hard_disk_list.Add("0025_388A_81BB_14B5.")   'Zweet kamer, Jeroen 

        user_list.Add("lennardh")
        hard_disk_list.Add("141190402709")       'VTK PC, Lennard Hubert

        user_list.Add("Peterdw")
        hard_disk_list.Add("134309552747")       'VTK PC, Peter de Wild

        user_list.Add("Jeffreyvdz")
        hard_disk_list.Add("ACE4_2E81_7006_2BD9.")     'VTK Laptop, Jeffrey van der Zwart

        user_list.Add("Twana")
        hard_disk_list.Add("ACE4_2E81_7006_2BD7.")     'VTK Laptop, Twan Akbheis

        user_list.Add("robru")
        hard_disk_list.Add("174741803447")     'VTK Laptop, Rob 

        nu = Now()
        nu2 = CDate("2019-12-01 00:00:00")
        life_time = CInt((nu2 - nu).TotalDays)
        Label101.Text = "Expire " & life_time.ToString

        TextBox28.Text = "Q" & Now.ToString("yy") & ".10"

        Pro_user = Environment.UserName     'User name on the screen
        HD_number = HardDisc_Id()           'Harddisk identification
        Me.Text &= "  (" & Pro_user & ")"

        'Check user name 
        For i = 0 To user_list.Count - 1
            If StrComp(LCase(Pro_user), LCase(user_list.Item(i))) = 0 Then pass_name = True
        Next

        'Check disc_id
        For i = 0 To hard_disk_list.Count - 1
            If CBool(HD_number = Trim(hard_disk_list(i))) Then pass_disc = True
        Next

        If pass_name = False Or pass_disc = False Then
            MessageBox.Show("VTK Cyclone selection program" & vbCrLf & "Access denied, contact GPa" & vbCrLf)
            MessageBox.Show("User_name= " & Pro_user & ", Pass name= " & pass_name.ToString)
            MessageBox.Show("HD_id= *" & HD_number & "*" & ", Pass disc= " & pass_disc.ToString)
            Environment.Exit(0)
        End If

        If life_time < 0 Then
            MessageBox.Show("Program lease Is Expired, contact GPa")
            Environment.Exit(0)
        End If

        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")

        'De weerstandscoefficient volgt uit het cycloon type
        weerstand_coef = {7, 7, 7, 7, 7.5, 9.5, 14.5}

        For hh = 0 To (cyl_dimensions.Length - 1)  'Fill combobox1 cyclone types
            words = cyl_dimensions(hh).Split(";")
            ComboBox1.Items.Add(words(0))
        Next hh
        ComboBox1.SelectedIndex = 5                 'Select Cyclone type
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles button1.Click, TabPage1.Enter, numericUpDown3.ValueChanged, numericUpDown2.ValueChanged, numericUpDown14.ValueChanged, NumericUpDown1.ValueChanged, CheckBox1.CheckedChanged, numericUpDown5.ValueChanged, NumericUpDown20.ValueChanged, NumericUpDown19.ValueChanged, NumericUpDown18.ValueChanged, ComboBox1.SelectedIndexChanged, numericUpDown9.ValueChanged, numericUpDown8.ValueChanged, numericUpDown7.ValueChanged, numericUpDown6.ValueChanged, numericUpDown12.ValueChanged, numericUpDown11.ValueChanged, numericUpDown10.ValueChanged, numericUpDown13.ValueChanged
        Get_input()
    End Sub
    Private Sub Get_input()
        Dim words() As String
        Dim cyl_dim(20), db As Double
        Dim Flow, delta_p, K_waarde As Double
        Dim ro_gas, ro_particle, visco, wc As Double
        Dim no_cycl As Double   'Number cyclones
        Dim stofb As Double
        Dim tot_kgh As Double       'Dust inlet per hour totaal 
        Dim kgh As Double           'Dust inlet per hour/cycloon 
        Dim kgs As Double           'Dust inlet per second

        Dim body_dia As Double      '[m]
        Dim in_hoog As Double       '[m]
        Dim in_breed As Double      '[m]
        Dim dia_outlet As Double    '[m] gas outlet

        Dim inlet_velos As Double   '[m/s]
        Dim outlet_velos As Double  '[m/s]

        Dim total_loss As Double    'Berekende verlies
        Dim class_loss As Double    'Loss in [kg] per class
        Dim effficiency As Double
        Dim total_input_weight As Double
        Dim dp50_dia As Double

        If (ComboBox1.SelectedIndex > -1) Then     'Prevent exceptions
            words = cyl_dimensions(ComboBox1.SelectedIndex).Split(";")
            For hh = 1 To 15
                cyl_dim(hh) = words(hh)         'Cyclone dimensions
            Next
            no_cycl = NumericUpDown20.Value     'Paralelle cyclonen
            db = numericUpDown5.Value           'Body diameter
            in_hoog = cyl_dim(1) * db           '[m]
            in_breed = cyl_dim(2) * db          '[m]
            dia_outlet = cyl_dim(6) * db        '[m] 
            body_dia = numericUpDown5.Value     '[m]
            Flow = NumericUpDown1.Value / 3600  '[m3/s]
            Flow /= no_cycl                     '[m3/s/cycloon]
            ro_gas = numericUpDown3.Value       '[kg/m3]
            ro_particle = numericUpDown2.Value  '[kg/m3]
            visco = numericUpDown14.Value       '[cPoise]
            stofb = NumericUpDown4.Value        '[g/Am3]

            '----------- inlaat snelheid ---------------------
            inlet_velos = Flow / (in_breed * in_hoog)

            '----------- uitlaat snelheid ---------------------
            outlet_velos = Flow / ((PI / 4) * dia_outlet ^ 2)   '[m/s]

            '----------- Pressure loss cyclone----------------------
            wc = weerstand_coef(ComboBox1.SelectedIndex)
            delta_p = 0.5 * ro_gas * inlet_velos ^ 2 * wc

            '----------- stof belasting ------------
            kgs = Flow * stofb / 1000               '[kg/s/cycloon]
            kgh = kgs * 3600                        '[kg/h/cycloon]
            tot_kgh = kgh * no_cycl                 '[kg/h] total

            '----------- K_waarde-----------------------------------
            K_waarde = db * 2000 * visco * 16 / (ro_particle * 0.0181 * inlet_velos)
            K_waarde = Sqrt(K_waarde)

            '----------- presenteren ----------------------------------
            TextBox36.Text = Flow.ToString("0.000")                 '[m3/s] flow

            '----------- presenteren afmetingen ------------------------------
            TextBox1.Text = (in_hoog).ToString("0.000")              'inlaat breedte
            TextBox2.Text = (in_breed).ToString("0.000")             'Inlaat hoogte
            TextBox3.Text = (cyl_dim(3) * db).ToString("0.000")      'Inlaat lengte
            TextBox4.Text = (cyl_dim(4) * db).ToString("0.000")      'Inlaat hartmaat
            TextBox5.Text = (cyl_dim(5) * db).ToString("0.000")      'Inlaat afschuining

            TextBox6.Text = (cyl_dim(6) * db).ToString("0.000")      'Uitlaat keeldia inw.
            TextBox7.Text = (cyl_dim(7) * db).ToString("0.000")      'Uitlaat flensdiameter inw.

            TextBox8.Text = (cyl_dim(8) * db).ToString("0.000")      'Lengte insteekpijp inw.

            TextBox9.Text = (cyl_dim(9) * db).ToString("0.000")      'Lengte romp + conus
            TextBox10.Text = (cyl_dim(10) * db).ToString("0.000")    'Lengte romp
            TextBox11.Text = (cyl_dim(11) * db).ToString("0.000")    'Lengte çonus

            TextBox12.Text = (cyl_dim(12) * db).ToString("0.000")    'Dia_conus / 3P-pijp
            TextBox13.Text = (cyl_dim(13) * db).ToString("0.000")    'Lengte 3P-pijp

            TextBox14.Text = (cyl_dim(14) * db).ToString("0.000")    'Dia_conus / 3P-pijp
            TextBox15.Text = (cyl_dim(15) * db).ToString("0.000")    'Lengte 3P-pijp

            TextBox16.Text = inlet_velos.ToString("0.0")            'inlaat snelheid
            TextBox17.Text = delta_p.ToString("0")                  'Pressure loss
            TextBox22.Text = outlet_velos.ToString("0.0")           'uitlaat snelheid

            TextBox23.Text = K_waarde.ToString("0.000")             'Stokes waarde tov Standaard cycloon
            TextBox37.Text = numericUpDown5.Value.ToString          'Cycloone dia_avemeter
            TextBox38.Text = ComboBox1.SelectedItem                 'Cycloon type

            Draw_chart()
            '---------- Check speed ---------------
            If inlet_velos < 12 Or inlet_velos > 25 Then
                TextBox16.BackColor = Color.Red
            Else
                TextBox16.BackColor = Color.LightGreen
            End If

            '--------- Inlet korrel-groep data -----------
            Init_groups()

            korrel_grp(0).class_wght_cum_pro = numericUpDown6.Value / 100  'Percentale van de inlaat stof belasting
            korrel_grp(1).class_wght_cum_pro = numericUpDown7.Value / 100
            korrel_grp(2).class_wght_cum_pro = numericUpDown8.Value / 100
            korrel_grp(3).class_wght_cum_pro = numericUpDown9.Value / 100
            korrel_grp(4).class_wght_cum_pro = numericUpDown10.Value / 100
            korrel_grp(5).class_wght_cum_pro = numericUpDown11.Value / 100
            korrel_grp(6).class_wght_cum_pro = numericUpDown12.Value / 100
            korrel_grp(7).class_wght_cum_pro = numericUpDown13.Value / 100

            '---- moet opgeteld 100% zijn --------------

            '---- determine group weights in [%] and [kg]-----------
            For h = 0 To 7
                korrel_grp(h).class_wght_pro = korrel_grp(h).class_wght_cum_pro - korrel_grp(h + 1).class_wght_cum_pro
                korrel_grp(h).class_wght_kg = korrel_grp(h).class_wght_pro * tot_kgh
            Next

            '--------- overall resultaat --------------------
            total_loss = 0
            class_loss = 0
            total_input_weight = 0

            For h = 0 To 7
                korrel_grp(h).verlies = Calc_verlies(korrel_grp(h).dia_ave, False)

                '--- write in dataview grid -----
                DataGridView1.Rows.Item(h).Cells(0).Value = korrel_grp(h).dia_small
                DataGridView1.Rows.Item(h).Cells(1).Value = korrel_grp(h).dia_big
                DataGridView1.Rows.Item(h).Cells(2).Value = korrel_grp(h).dia_ave

                DataGridView1.Rows.Item(h).Cells(3).Value = korrel_grp(h).class_wght_kg.ToString("0") '[kg/h]
                DataGridView1.Rows.Item(h).Cells(4).Value = (korrel_grp(h).class_wght_pro * 100).ToString("0.0") '[%]
                DataGridView1.Rows.Item(h).Cells(5).Value = (korrel_grp(h).verlies * 100).ToString("0.0") '[%]
                class_loss = (korrel_grp(h).class_wght_kg * korrel_grp(h).verlies)
                DataGridView1.Rows.Item(h).Cells(6).Value = class_loss.ToString("0.0") '[kg/hr]

                total_loss += class_loss
                total_input_weight += korrel_grp(h).class_wght_kg
            Next h
            DataGridView1.Rows.Item(8).Cells(6).Value = total_loss.ToString("0.0")
            DataGridView1.Rows.Item(8).Cells(3).Value = total_input_weight.ToString("0")
            DataGridView1.AutoResizeColumns()

            '---------- efficiency -----------
            effficiency = ((tot_kgh - total_loss) / tot_kgh) * 100  '[%]

            '---------- Calc diameter with 50% separation ---
            dp50_dia = Dp50()

            '---------- present -------
            TextBox26.Text = dp50_dia.ToString("0.00")   '[mu]

            TextBox39.Text = kgh.ToString("0")          'Stof inlet
            TextBox40.Text = tot_kgh.ToString("0")  'Stof inlet totaal
            TextBox25.Text = effficiency.ToString("0.0")
        End If
    End Sub
    Private Sub Init_groups()
        DataGridView1.ColumnCount = 7
        DataGridView1.Rows.Clear()
        DataGridView1.Rows.Add(8)
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        '[mu] Class lower particle diameter limit diameter
        korrel_grp(0).dia_small = 0
        korrel_grp(1).dia_small = 10
        korrel_grp(2).dia_small = 15
        korrel_grp(3).dia_small = 20
        korrel_grp(4).dia_small = 30
        korrel_grp(5).dia_small = 40
        korrel_grp(6).dia_small = 50
        korrel_grp(7).dia_small = 60

        '[mu] Class upper particle diameter limit diameter
        korrel_grp(0).dia_big = 10
        korrel_grp(1).dia_big = 15
        korrel_grp(2).dia_big = 20
        korrel_grp(3).dia_big = 30
        korrel_grp(4).dia_big = 40
        korrel_grp(5).dia_big = 50
        korrel_grp(6).dia_big = 60
        korrel_grp(7).dia_big = 80

        For h = 0 To 7
            korrel_grp(h).dia_ave = (korrel_grp(h).dia_small + korrel_grp(h).dia_big) / 2
        Next
        DataGridView1.Columns(0).HeaderText = "Dia lower [mu]"
        DataGridView1.Columns(1).HeaderText = "Dia upper [mu]"
        DataGridView1.Columns(2).HeaderText = "Dia average [mu]"
        DataGridView1.Columns(3).HeaderText = "Weight [kg/h]"
        DataGridView1.Columns(4).HeaderText = "Weight [%]"
        DataGridView1.Columns(5).HeaderText = "Loss [%]"
        DataGridView1.Columns(6).HeaderText = "Loss [kg/h]"
    End Sub
    '-------- Bereken het verlies getal -----------
    '----- de input is de korrel grootte-----------
    Private Function Calc_verlies(korrel_g As Double, present As Boolean)
        Dim words() As String
        Dim dia_krit, fac_m, fac_a, fac_k, kwaarde As Double
        Dim verlies As Double = 1

        If (ComboBox1.SelectedIndex > -1) Then
            Double.TryParse(TextBox23.Text, kwaarde)

            '-------------- korrelgrootte factoren ------
            words = rekenlijnen(ComboBox1.SelectedIndex).Split(";")

            dia_krit = words(1)

            '-------- de grafieken zijn in 2 delen gesplits voor hogere nauwkeurigheid----------
            If korrel_g < dia_krit Then
                fac_m = words(2)
                fac_k = words(3)
                fac_a = words(4)
            Else
                fac_m = words(5)
                fac_k = words(6)
                fac_a = words(7)
            End If

            '------ loss calculation ----
            verlies = (((korrel_g / kwaarde) - fac_m) / fac_k) ^ fac_a
            verlies = Math.E ^ -verlies

            If present Then
                '---------- present------------------
                TextBox18.Text = dia_krit.ToString("0.00")          'diameter_kritisch
                TextBox19.Text = fac_m.ToString("0.00")             'faktor-m
                TextBox20.Text = fac_k.ToString("0.00")             'faktor-kappa
                TextBox21.Text = fac_a.ToString("0.00")             'faktor-a
                TextBox27.Text = (verlies * 100).ToString("0.0") 'verlies
            End If
        End If
        Return (verlies)
    End Function
    'Note dp(50) meaning with this diameter 50% is separated and 50% is lost
    Private Function Dp50() As Double
        Dim dia_dp50 As Double
        Dim los As Double

        '----- now finf 50% loss --------------------------
        For dia_dp50 = 1 To 10 Step 0.1      'Particle diameter [mu]
            los = Calc_verlies(dia_dp50, False) 'Loss [%]
            If los <= 0.5 Then
                Exit For
            End If
        Next
        Return (dia_dp50)
    End Function


    Private Sub Draw_chart()
        '-------
        Dim s_points(100, 2) As Double
        Dim h As Integer

        Chart1.Series.Clear()
        Chart1.ChartAreas.Clear()
        Chart1.Titles.Clear()
        Chart1.ChartAreas.Add("ChartArea0")

        Chart1.Series.Add("Series" & h.ToString)
        Chart1.Series(h).ChartArea = "ChartArea0"
        Chart1.Series(h).ChartType = DataVisualization.Charting.SeriesChartType.Line
        Chart1.Series(h).BorderWidth = 2
        Chart1.Series(h).IsVisibleInLegend = False

        Chart1.Titles.Add("Verlies Curve")
        Chart1.ChartAreas("ChartArea0").AxisX.Title = "particle dia [mu]"

        Chart1.ChartAreas("ChartArea0").AxisY.Title = "Loss [%] (niet gevangen)"
        Chart1.ChartAreas("ChartArea0").AxisY.Minimum = 0       'Loss
        Chart1.ChartAreas("ChartArea0").AxisY.Maximum = 100     'Loss
        Chart1.ChartAreas("ChartArea0").AxisY.Interval = 10     'Interval
        Chart1.ChartAreas("ChartArea0").AxisX.MinorTickMark.Enabled = True
        Chart1.ChartAreas("ChartArea0").AxisY.MinorTickMark.Enabled = True
        Chart1.ChartAreas("ChartArea0").AxisX.MinorGrid.Enabled = True
        Chart1.ChartAreas("ChartArea0").AxisY.MinorGrid.Enabled = True

        If CheckBox1.Checked Then
            Chart1.ChartAreas("ChartArea0").AxisX.IsLogarithmic = True
            Chart1.ChartAreas("ChartArea0").AxisX.Minimum = 1     'Particle size
            Chart1.ChartAreas("ChartArea0").AxisX.Maximum = 100   'Particle size
        Else
            Chart1.ChartAreas("ChartArea0").AxisX.IsLogarithmic = False
            Chart1.ChartAreas("ChartArea0").AxisX.Minimum = 0     'Particle size
            Chart1.ChartAreas("ChartArea0").AxisX.Maximum = 40    'Particle size
        End If

        '----- now calc chart poins --------------------------
        For h = 0 To 100
            s_points(h, 0) = h                                   'Particle diameter [mu]
            s_points(h, 1) = Calc_verlies(s_points(h, 0), False) * 100  'Loss [%]
        Next

        '------ now present-------------
        For h = 0 To 40 - 1   'Fill line chart
            Chart1.Series(0).Points.AddXY(s_points(h, 0), s_points(h, 1))
        Next h
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click, NumericUpDown15.ValueChanged
        Dim pdia As Double

        pdia = NumericUpDown15.Value
        Calc_verlies(pdia, True)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If TextBox28.Text.Trim.Length > 0 And TextBox29.Text.Trim.Length > 0 Then
            Save_tofile()
        Else
            MessageBox.Show("Complete Quote and Tag number")
        End If
    End Sub
    Private Sub Save_tofile()

        Dim temp_string, user As String

        user = Trim(Environment.UserName)         'User name on the screen
        Dim filename As String = "Cyclone_select_" & TextBox28.Text & "_" & TextBox29.Text & DateTime.Now.ToString("_yyyy_MM_dd_") & user & ".vtk2"
        filename = Replace(filename, Chr(32), Chr(95)) 'Replace the space's

        Dim all_num, all_combo, all_check, all_radio As New List(Of Control)
        Dim i As Integer

        If String.IsNullOrEmpty(TextBox29.Text) Then
            TextBox29.Text = "-"
        End If

        '-------- Project information -----------------
        temp_string = TextBox28.Text & ";" & TextBox29.Text & ";"
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        '-------- find all numeric controls -----------------
        FindControlRecursive(all_num, Me, GetType(NumericUpDown))   'Find the control
        all_num = all_num.OrderBy(Function(x) x.Name).ToList()      'Alphabetical order
        For i = 0 To all_num.Count - 1
            Dim grbx As NumericUpDown = CType(all_num(i), NumericUpDown)
            temp_string &= grbx.Value.ToString & ";"
            TextBox24.Text &= grbx.Name.ToString & "value= " & grbx.Value.ToString & vbTab & " is Saved to file" & vbCrLf
        Next
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        '-------- find all combobox controls and save ----------------
        FindControlRecursive(all_combo, Me, GetType(ComboBox))      'Find the control
        all_combo = all_combo.OrderBy(Function(x) x.Name).ToList()   'Alphabetical order
        For i = 0 To all_combo.Count - 1
            Dim grbx As ComboBox = CType(all_combo(i), ComboBox)
            temp_string &= grbx.SelectedItem.ToString & ";"
        Next
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        '-------- find all checkbox controls and save --------------------
        FindControlRecursive(all_check, Me, GetType(CheckBox))      'Find the control
        all_check = all_check.OrderBy(Function(x) x.Name).ToList()  'Alphabetical order
        For i = 0 To all_check.Count - 1
            Dim grbx As CheckBox = CType(all_check(i), CheckBox)
            temp_string &= grbx.Checked.ToString & ";"
        Next
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        '-------- find all radio controls and save ---------------
        FindControlRecursive(all_radio, Me, GetType(RadioButton))   'Find the control
        all_radio = all_radio.OrderBy(Function(x) x.Name).ToList()  'Alphabetical order
        For i = 0 To all_radio.Count - 1
            Dim grbx As RadioButton = CType(all_radio(i), RadioButton)
            temp_string &= grbx.Checked.ToString & ";"
        Next
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        '---- if path not exist then create one----------
        Try
            If (Not System.IO.Directory.Exists(dirpath_tmp)) Then System.IO.Directory.CreateDirectory(dirpath_tmp)
        Catch ex As Exception
            MessageBox.Show("Create directory without VTK intranet (L578)" & vbCrLf & ex.Message)
        End Try

        Try
            If (Not System.IO.Directory.Exists(dirpath_Temp)) Then System.IO.Directory.CreateDirectory(dirpath_Temp)
            If (Not System.IO.Directory.Exists(dirpath_Eng)) Then System.IO.Directory.CreateDirectory(dirpath_Eng)
            If (Not System.IO.Directory.Exists(dirpath_Rap)) Then System.IO.Directory.CreateDirectory(dirpath_Rap)
        Catch ex As Exception
            MessageBox.Show("Can not create directory on the VTK intranet (L6286) " & vbCrLf & vbCrLf & ex.Message)
        End Try

        Try
            If CInt(temp_string.Length.ToString) > 100 Then      'String may be empty
                If Directory.Exists(dirpath_Eng) Then
                    File.WriteAllText(dirpath_Eng & filename, temp_string, Encoding.ASCII)     'used at VTK with intranet
                Else
                    File.WriteAllText(dirpath_tmp & filename, temp_string, Encoding.ASCII)     'used at home
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Line 6298, " & ex.Message)  ' Show the exception's message.
        End Try
    End Sub
    'Retrieve control settings and case_x_conditions from file
    'Split the file string into 5 separate strings
    'Each string represents a control type (combobox, checkbox,..)
    'Then split up the secton string into part to read into the parameters
    Private Sub Read_file()
        Dim control_words(), words() As String
        Dim i As Integer
        Dim ttt As Double
        Dim k As Integer = 0
        Dim all_num, all_combo, all_check, all_radio As New List(Of Control)
        Dim separators() As String = {";"}
        Dim separators1() As String = {"BREAK"}

        OpenFileDialog1.FileName = "Cyclone_select_*"

        If Directory.Exists(dirpath_Eng) Then
            OpenFileDialog1.InitialDirectory = dirpath_Eng  'used at VTK
        Else
            OpenFileDialog1.InitialDirectory = dirpath_tmp  'used at home
        End If

        OpenFileDialog1.Title = "Open a Text File"
        OpenFileDialog1.Filter = "VTK2 Files|*.vtk2|VTK1 file|*.vtk"
        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim readText As String = File.ReadAllText(OpenFileDialog1.FileName, Encoding.ASCII)
            control_words = readText.Split(separators1, StringSplitOptions.None) 'Split the read file content
            '----- retrieve Project information ----------------------
            words = control_words(0).Split(separators, StringSplitOptions.None) 'Split the read file content
            TextBox28.Text = words(0)                  'Project number
            TextBox29.Text = words(1)                  'Tag no

            '---------- Retrieve Numeric controls from disk-----------------
            FindControlRecursive(all_num, Me, GetType(NumericUpDown))               'Find the numericupdowns
            all_num = all_num.OrderBy(Function(x) x.Name).ToList()                  'Sort in Alphabetical order
            words = control_words(1).Split(separators, StringSplitOptions.None)     'Split the read file content
            For i = 0 To all_num.Count - 1
                Dim grbx As NumericUpDown = CType(all_num(i), NumericUpDown)
                '--- dit deel voorkomt problemen bij het uitbreiden van het aantal checkboxes--
                If (i < words.Length - 1) Then
                    If Not (Double.TryParse(words(i + 1), ttt)) Then
                        MessageBox.Show("Numeric controls conversion problem occured")
                        TextBox24.Text &= grbx.Name.ToString & ", ttt= |" & ttt.ToString & "| Cannot be converted to number" & vbCrLf
                    End If

                    If ttt <= grbx.Maximum And ttt >= grbx.Minimum Then
                        grbx.Value = CDec(ttt)          'OK
                    Else
                        TextBox24.Text &= grbx.Name.ToString & " ttt= " & ttt.ToString & " range=, " & grbx.Minimum & "-" & grbx.Maximum & " Minimum value Is used " & vbCrLf
                        grbx.Value = grbx.Minimum       'NOK
                    End If
                Else
                    TextBox24.Text &= "Warning last Numeric-Updown-controls Not found In file" & vbCrLf
                End If
            Next

            '---------- Retrieve  combobox controls -----------------
            FindControlRecursive(all_combo, Me, GetType(ComboBox))
            all_combo = all_combo.OrderBy(Function(x) x.Name).ToList()          'Alphabetical order
            words = control_words(2).Split(separators, StringSplitOptions.None) 'Split the read file content
            For i = 0 To all_combo.Count - 1
                Dim grbx As ComboBox = CType(all_combo(i), ComboBox)
                '--- dit deel voorkomt problemen bij het uitbreiden van het aantal checkboxes--
                If (i < words.Length - 1) Then
                    grbx.SelectedItem = words(i + 1)
                Else
                    TextBox24.Text &= "Warning last combobox Not found In file" & vbCrLf
                End If
            Next

            '---------- Retrieve  checkbox controls -----------------
            FindControlRecursive(all_check, Me, GetType(CheckBox))
            all_check = all_check.OrderBy(Function(x) x.Name).ToList()                  'Alphabetical order
            words = control_words(3).Split(separators, StringSplitOptions.None) 'Split the read file content
            For i = 0 To all_check.Count - 1
                Dim grbx As CheckBox = CType(all_check(i), CheckBox)
                '--- dit deel voorkomt problemen bij het uitbreiden van het aantal checkboxes--
                If (i < words.Length - 1) Then
                    Boolean.TryParse(words(i + 1), grbx.Checked)
                Else
                    TextBox24.Text &= "Warning last checkbox Not found In file" & vbCrLf
                End If
            Next

            '---------- Retrieve  radiobuttons controls -----------------
            FindControlRecursive(all_radio, Me, GetType(RadioButton))
            all_radio = all_radio.OrderBy(Function(x) x.Name).ToList()                  'Alphabetical order
            words = control_words(4).Split(separators, StringSplitOptions.None) 'Split the read file content
            For i = 0 To all_radio.Count - 1
                Dim grbx As RadioButton = CType(all_radio(i), RadioButton)
                '--- dit deel voorkomt problemen bij het uitbreiden van het aantal radiobuttons--
                If (i < words.Length - 1) Then
                    Boolean.TryParse(words(i + 1), grbx.Checked)
                Else
                    TextBox24.Text &= "Warning last radiobutton Not found In file" & vbCrLf
                End If
            Next
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Read_file()
    End Sub
    '----------- Find all controls on form1------
    'Nota Bene, sequence of found control may be differen, List sort is required
    Public Shared Function FindControlRecursive(ByVal list As List(Of Control), ByVal parent As Control, ByVal ctrlType As System.Type) As List(Of Control)
        If parent Is Nothing Then Return list

        If parent.GetType Is ctrlType Then
            list.Add(parent)
        End If
        For Each child As Control In parent.Controls
            FindControlRecursive(list, child, ctrlType)
        Next
        Return list
    End Function

    Public Function HardDisc_Id() As String
        'Add system.management as reference !!
        'imports system.management
        Dim tmpStr2 As String = ""
        Dim myScop As New ManagementScope("\\" & Environment.MachineName & "\root\cimv2")
        Dim oQuer As New SelectQuery("SELECT * FROM WIN32_DiskDrive")

        Dim oResult As New ManagementObjectSearcher(myScop, oQuer)
        Dim oIte As ManagementObject
        Dim oPropert As PropertyData
        For Each oIte In oResult.Get()
            For Each oPropert In oIte.Properties
                If Not oPropert.Value Is Nothing AndAlso oPropert.Name = "SerialNumber" Then
                    tmpStr2 = oPropert.Value.ToString
                    Exit For
                End If
            Next
            Exit For
        Next
        Return (Trim(tmpStr2))         'Harddisk identification
    End Function
End Class
