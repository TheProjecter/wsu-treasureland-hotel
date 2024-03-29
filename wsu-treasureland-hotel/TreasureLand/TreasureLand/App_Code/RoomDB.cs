﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;


namespace TreasureLand.App_Code
{
    /// <summary>
    /// Summary description for RoomDB
    /// </summary>
    public static partial class RoomDB
    {
        //private static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=Ghana_Hotel;Integrated Security=True";
        private static IFormatProvider dateFormat = new System.Globalization.CultureInfo("en-GB");

        /// <summary>
        /// Returns the corresponding connection string
        /// </summary>
        /// <returns></returns>
        private static string getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;
        }

        /// <summary>
        /// Counts the total number of rooms currently in the database
        /// </summary>
        /// <returns>The total number of rooms in the database</returns>
        public static int countRooms()
        {
            SqlConnection conn = new SqlConnection(getConnectionString());

            try
            {
                conn.Open(); //Open the connection

                string command = "SELECT COUNT(RoomID) FROM Room";

                SqlCommand connCommand = new SqlCommand(command, conn);

                SqlDataReader rt = connCommand.ExecuteReader();
                rt.Read();
                int val = Int32.Parse(rt[0].ToString()); //Only one item will be returned
                rt.Close();

                return val;
            }
            catch (Exception e)
            {
            }
            return 0;
        }

        /// <summary>
        /// Because of the nature of the hashtable, a list of all of the
        /// roomNumbers must be returned to generate a complete table even
        /// if a room lacks any records for it
        /// </summary>
        /// <returns>Linked list of all of the room names</returns>
        public static List<string[]> getRoomNumbers()
        {
            SqlConnection conn = new SqlConnection(getConnectionString());

            try
            {
                conn.Open(); //Open the connection

                string command = "SELECT room.RoomNumbers, rtype.RoomType, room.RoomStatus " +
                                 "FROM Room room " +
                                 "INNER JOIN HotelRoomType rtype ON rtype.HotelRoomTypeID = room.HotelRoomTypeID " +
                                 "ORDER BY RoomNumbers";

                SqlCommand connCommand = new SqlCommand(command, conn);

                SqlDataReader rt = connCommand.ExecuteReader();

                List<string[]> rooms = new List<string[]>();
                while (rt.Read())
                {
                    string[] room = new string[3];
                    room[0] = rt[0].ToString();
                    room[1] = rt[1].ToString();
                    room[2] = Char.Parse(rt[2].ToString()) == 'M' ? "M" : "A";
                    rooms.Add(room);
                }
                rt.Close();

                sortRoomsNumerically(rooms);
                return rooms;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        /// <summary>
        /// Gets all of the room types from the database to display in a drop down list
        /// </summary>
        /// <returns>A list of room types</returns>
        public static SqlDataReader getRoomTypes()
        {
            SqlConnection conn = new SqlConnection(getConnectionString());

            try
            {
                conn.Open(); //Open the connection

                string command = "SELECT RoomType, HotelRoomTypeID FROM HotelRoomType";

                SqlCommand connCommand = new SqlCommand(command, conn);

                SqlDataReader rt = connCommand.ExecuteReader();

                return rt;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        /// <summary>
        /// Retrieves all reservations in the database and packages
        /// </summary>
        /// <param name="beginRange">Ending dates before this time will be skipped</param>
        /// <returns>Beginning dates after this time will be skipped</returns>
        public static List<Row> getReservations(DateTime beginRange, DateTime endRange)
        {
            SqlConnection conn = new SqlConnection(getConnectionString());

            try
            {
                conn.Open(); //Open the connection

                string command = "SELECT g.GuestFirstName, g.GuestSurName, rd.CheckinDate, res.ReservationStatus, rd.Nights, " +
                                 "room.RoomID, room.RoomNumbers, res.ReservationID, " +
                                 "rtype.HotelRoomTypeID, rtype.RoomType, rd.ReservationDetailID " +
                                 "FROM Guest g " +
                                 "INNER JOIN Reservation res ON res.GuestID = g.GuestID " +
                                 "INNER JOIN ReservationDetail rd ON res.ReservationID = rd.ReservationID " +
                                 "INNER JOIN Room room ON rd.RoomID = room.RoomID " +
                                 "INNER JOIN HotelRoomType rtype ON rtype.HotelRoomTypeID = room.HotelRoomTypeID " +
                                 "ORDER BY room.RoomNumbers";

                SqlCommand connCommand = new SqlCommand(command, conn);

                SqlDataReader rt = connCommand.ExecuteReader();

                List<Row> RT = new List<Row>();

                while (rt.Read())
                {
                    string dateTemp = rt[2].ToString();
                    //string debug = dateTemp.Substring(0, dateTemp.IndexOf(' '));
                    DateTime begin = DateTime.Parse(dateTemp.Substring(0,dateTemp.IndexOf(' ')));
                    DateTime end = begin.AddDays(Double.Parse(rt[4].ToString()));
                    //if ((end - beginRange).Days < 0 || (begin - endRange).Days > 0)
                        //continue; //Skip the rest and go to the next record

                    char resStatus = Char.Parse(rt[3].ToString());
                    short id = Int16.Parse(rt[5].ToString());
                    string roomNumber = rt[6].ToString();
                    int resID = Int32.Parse(rt[7].ToString());
                    int hotelRoomTypeID = Int32.Parse(rt[8].ToString());
                    string roomType = rt[9].ToString();
                    int reservationDetailsID = Int32.Parse(rt[10].ToString());
                    string guestName = rt[0].ToString() + ' ' + rt[1].ToString();
                    //bool roomStatus = Char.Parse(rt[9].ToString()) == 'M' ? true : false;

                    Row temp = new Row(id, resID, hotelRoomTypeID, reservationDetailsID); //Create and fill a Row object
                    temp.Begin = begin;
                    temp.End = end;
                    temp.ReservationType = resStatus;
                    temp.RoomNumber = roomNumber;
                    temp.RoomType = roomType;
                    temp.GuestName = guestName;
                    //temp.Maintenance = roomStatus;

                    RT.Add(temp); //Add it to the list
                }

                return RT;
            }
            catch (Exception e)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets a RoomID based on a RoomNumber. Assuming no RoomNumber duplicates
        /// </summary>
        /// <param name="RoomName">Name/Numbers of the Room</param>
        /// <returns>The ID of the Room</returns>
        public static int getRoomId(string RoomName)
        {
            SqlConnection conn = new SqlConnection(getConnectionString());

            try
            {
                conn.Open(); //Open the connection

                string command = "SELECT RoomID FROM Room " + 
                                 "WHERE RoomNumbers = @RoomNumbers";

                SqlCommand connCommand = new SqlCommand(command, conn);

                connCommand.Parameters.AddWithValue("@RoomNumbers", RoomName);

                SqlDataReader rt = connCommand.ExecuteReader();

                rt.Read();
                int RT = Int32.Parse(rt[0].ToString());
                rt.Close();

                return RT;
            }
            catch (Exception e)
            {
            }
            return 0;
        }

        /// <summary>
        /// Updates a ReservationDetail by moving it to a new room
        /// </summary>
        /// <param name="ReservationID">ID of the reservation detail to change</param>
        /// <param name="ReservationDetailID">Specific ID for the detail to change the reservation date</param>
        /// <param name="RoomID">ID of the room to change to</param>
        /// <returns>Number of rows affected</returns>
        public static int updateRoom(int ReservationID, int ReservationDetailID, int RoomID, DateTime time, int nights)
        {
            SqlConnection conn = new SqlConnection(getConnectionString());

            try
            {
                conn.Open(); //Open the connection

                string command = "UPDATE [ReservationDetail] SET [RoomID] = @RoomID, " +
                                 "[Nights] = @Nights " +
                                 "WHERE [ReservationDetailID] = @ReservationDetailID";

                SqlCommand connCommand = new SqlCommand(command, conn);

                connCommand.Parameters.AddWithValue("@RoomID", RoomID);
                connCommand.Parameters.AddWithValue("@ReservationDetailID", ReservationDetailID);
                connCommand.Parameters.AddWithValue("@Nights", nights);

                int ReservationDetail = connCommand.ExecuteNonQuery();
                if (ReservationDetail <= 0)
                    return ReservationDetail; //An error occurred
                command = "UPDATE [Reservation] SET [ReservationDate] = @ReservationDate " +
                          "WHERE [ReservationID] = @ReservationID";

                connCommand = new SqlCommand(command, conn);

                connCommand.Parameters.AddWithValue("@ReservationDate", time);
                connCommand.Parameters.AddWithValue("@ReservationID", ReservationID);

                ReservationDetail = connCommand.ExecuteNonQuery();

                return ReservationDetail;
            }
            catch (Exception e)
            {
            }
            return 0;
        }
       
        /// <summary>
        /// Searches for a current guest based on the entered information.
        /// </summary>
        /// <param name="FirstName">First Name of guest</param>
        /// <param name="SurName">SurName of guest</param>
        /// <param name="ReservationID">Reservation ID numbmer</param>
        /// <param name="RoomNumber">Room Number</param>
        /// <returns></returns>
        public static IEnumerable LocateGuestRoom(string FirstName, string SurName, string ReservationID, int RoomID)
        {
            SqlConnection con = new SqlConnection(getConnectionString());
            string sel =
                "SELECT Reservation.ReservationID, Guest.GuestFirstName, Guest.GuestSurName, ReservationDetail.RoomID FROM Reservation INNER JOIN Guest ON Reservation.GuestID = Guest.GuestID INNER JOIN ReservationDetail ON Reservation.ReservationID = ReservationDetail.ReservationID "+
                "WHERE Guest.GuestFirstName = '" + FirstName + "' OR Guest.GuestSurName = '" + SurName +"' OR Reservation.ReservationID = '" + ReservationID + "' OR ReservationDetail.RoomID = '" + RoomID + "'";
            SqlCommand cmd =
            new SqlCommand(sel, con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        /// <summary>
        /// Sorts the room list by number rather than alphabetically
        /// </summary>
        /// <param name="rooms">List of all of the room names</param>
        /// <returns>List of room names sorted</returns>
        private static void sortRoomsNumerically(List<string[]> rooms)
        {
            if (rooms.Count < 2)
                return; //Already sorted

            for(int i = 0; i < rooms.Count; i++)
                for (int j = i; j < rooms.Count; j++)
                {
                    string[] temp1 = rooms[i];
                    string[] temp2 = rooms[j];

                    if (temp1[0].Length > temp2[0].Length) //Sort by length
                    {
                        rooms[i] = temp2;
                        rooms[j] = temp1;
                    }
                    else if (temp1[0].CompareTo(temp2[0]) > 0) //Sort by value
                    {
                        rooms[i] = temp2;
                        rooms[j] = temp1;
                    }
                }
        }
    }
}