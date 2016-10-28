﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

/// <summary>
/// In charge of 
/// 1) keeping track of time
/// 2) keeping track of future events
/// 3) kickstart events
/// 4) receiving logs about everything 
/// 
/// </summary>
public static class GlobalRegistry
{
    static Dictionary<long, StudentData> studentData;//time that a particular student starts to search for a table

    static List<float> time;
    static List<int> seatsUsed;
    static List<int> seatsReserved;

    public static void initialize(List<Table> ts)
    {
        studentData = new Dictionary<long, StudentData>();
        time = new List<float>();
        seatsUsed = new List<int>();
        seatsReserved = new List<int>();

        //TODO: adapt to initial conditions
        time.Add(0);
        seatsUsed.Add(0);
        seatsReserved.Add(0);
    }

    public static void updateTableData(int deltaUsed, int deltaReserved)
    {
        time.Add(GlobalEventManager.currentTime);
        seatsReserved.Add(seatsReserved.Last() + deltaReserved);
        seatsUsed.Add(seatsUsed.Last() + deltaUsed);
    }

    //Output Data upon termination
    public static void output()
    {
        string config = "TAKER_" + GlobalConstants.TABLE_TAKER_RATIO.ToString("0.00") + "_SHARER_" + GlobalConstants.TABLE_SHARER_RATIO + "_";
        string path = Utility.Path + "/Output/";
        //Directory.CreateDirectory(Path.GetDirectoryName(path));
        string pathStudent = path + "STUDE_DATA_" + config + System.Guid.NewGuid() + ".csv";
        string pathTable = path + "TABLE_DATA_" + config + System.Guid.NewGuid() + ".csv";

        //ID, System Time, Search Time
        string[] studentStringData = studentData.Select(s => s.Key + "," + s.Value.systemTime + "," + s.Value.searchTime).ToArray();
        File.WriteAllLines(pathStudent, studentStringData);

        string[] tableStringData = Enumerable.Range(0, time.Count - 1)
            .Select(i => time.ElementAt(i) + "," + seatsUsed.ElementAt(i) + "," + seatsReserved.ElementAt(i)).ToArray();
        File.WriteAllLines(pathTable, tableStringData);
    }
    
    public static void signalStudent(Student s)
    {
        float searchTime = (s.searchEnd - s.searchStart);
        float systemTime = s.leaveSystem - s.enterSystem;
        if (searchTime > 0)
        {
            Debug.Log("Student: " + s.ID + " has searched for " + searchTime);
        }
        studentData.Add(s.ID, new StudentData(searchTime, systemTime));
    }
}

//This is for all students
class StudentData
{
    public readonly float searchTime;
    public readonly float systemTime;

    public StudentData(float search, float system)
    {
        searchTime = search;
        systemTime = system;
    }
}

