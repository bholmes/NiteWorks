// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharp.Data


[<Literal>] 
let xmlIn =
            """<response xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="1.2" xsi:noNamespaceSchemaLocation="http://www.aviationweather.gov/static/adds/schema/metar1_2.xsd">""" +
            """<request_index>44314176</request_index>""" +
            """<data_source name="metars" />""" +
            """<request type="retrieve" />""" +
            """<errors />""" +
            """<warnings />""" +
            """<time_taken_ms>69</time_taken_ms>""" +
            """<data num_results="413">""" +
            """<METAR>""" +
            """<raw_text>KPIT 041851Z 21009KT 10SM FEW024 OVC033 06/00 A2992 RMK AO2 SLP147 T00560000</raw_text>""" +
            """<station_id>KPIT</station_id>""" +
            """<observation_time>2020-12-04T18:51:00Z</observation_time>""" +
            """<latitude>40.47</latitude>""" +
            """<longitude>-80.2</longitude>""" +
            """<temp_c>5.6</temp_c>""" +
            """<dewpoint_c>0.0</dewpoint_c>""" +
            """<wind_dir_degrees>210</wind_dir_degrees>""" +
            """<wind_speed_kt>9</wind_speed_kt>""" +
            """<visibility_statute_mi>10.0</visibility_statute_mi>""" +
            """<altim_in_hg>29.920275</altim_in_hg>""" +
            """<sea_level_pressure_mb>1014.7</sea_level_pressure_mb>""" +
            """<quality_control_flags>""" +
            """<auto_station>TRUE</auto_station>""" +
            """</quality_control_flags>""" +
            """<sky_condition sky_cover="FEW" cloud_base_ft_agl="2400" />""" +
            """<sky_condition sky_cover="OVC" cloud_base_ft_agl="3300" />""" +
            """<flight_category>VFR</flight_category>""" +
            """<metar_type>METAR</metar_type>""" +
            """<elevation_m>339.0</elevation_m>""" +
            """</METAR>""" +
            """<METAR>""" +
            """<raw_text>KPIT 041851Z 21009KT 10SM FEW024 OVC033 06/00 A2992 RMK AO2 SLP147 T00560000</raw_text>""" +
            """<station_id>KPIT</station_id>""" +
            """<observation_time>2020-12-04T18:51:00Z</observation_time>""" +
            """<latitude>40.47</latitude>""" +
            """<longitude>-80.2</longitude>""" +
            """<temp_c>5.6</temp_c>""" +
            """<dewpoint_c>0.0</dewpoint_c>""" +
            """<wind_dir_degrees>210</wind_dir_degrees>""" +
            """<wind_speed_kt>9</wind_speed_kt>""" +
            """<visibility_statute_mi>10.0</visibility_statute_mi>""" +
            """<altim_in_hg>29.920275</altim_in_hg>""" +
            """<sea_level_pressure_mb>1014.7</sea_level_pressure_mb>""" +
            """<quality_control_flags>""" +
            """<auto_station>TRUE</auto_station>""" +
            """</quality_control_flags>""" +
            """<sky_condition sky_cover="FEW" cloud_base_ft_agl="2400" />""" +
            """<sky_condition sky_cover="OVC" cloud_base_ft_agl="3300" />""" +
            """<flight_category>VFR</flight_category>""" +
            """<metar_type>METAR</metar_type>""" +
            """<elevation_m>339.0</elevation_m>""" +
            """</METAR>""" +
            """</data>""" +
            """</response>"""

type Response = XmlProvider<xmlIn>

let filterMetar (metar:Response.Metar) =
    match metar.ObservationTime.ToLocalTime().Hour with
        | 8 | 14 | 18 -> true
        | _ -> false

let ToF (cIn:decimal)=
    (cIn * 9.0m/5.0m) + 32.0m

let windDirection (direction:int) =
    match direction with
        | dir when dir >= 0 && dir <= 10 -> sprintf "%i  N" direction
        | dir when dir > 10 && dir < 80 -> sprintf "%i  NE" direction
        | dir when dir >= 80 && dir <= 100 -> sprintf "%i  E" direction
        | dir when dir > 100 && dir < 170 -> sprintf "%i  SE" direction
        | dir when dir >= 170 && dir <= 190 -> sprintf "%i  S" direction
        | dir when dir > 190 && dir < 260 -> sprintf "%i  SW" direction
        | dir when dir >= 260 && dir <= 280 -> sprintf "%i  W" direction
        | dir when dir > 280 && dir < 350 -> sprintf "%i  NW" direction
        | dir when dir >= 350 && dir <= 360 -> sprintf "%i  N" direction
        |_ -> sprintf "%i" direction



[<EntryPoint>]
let main argv =
    printfn "Hello"

    let query = "https://aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&stationString=KPIT&hoursBeforeNow=2000"

    let sample = Response.Load(query)

    printfn "| %-30s | %-10s | %-15s | %-20s |" "Date Time" "Temp" "Wind Direction" "Barometric Pressure"

    let filteredList = sample.Data.Metars |> Array.filter filterMetar |> Array.sortWith (fun elem1 elem2 -> if elem1.ObservationTime < elem2.ObservationTime then -1; else 1;)

    for metar in filteredList  do
        printfn "| %-30s | %-10f | %-15s | %-20f |" (metar.ObservationTime.ToLocalTime().ToString()) (ToF metar.TempC) (windDirection metar.WindDirDegrees) (metar.AltimInHg)

    
    0





