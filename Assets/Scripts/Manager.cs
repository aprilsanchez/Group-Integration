using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Manager : MonoBehaviour
{
    public int season = 6;
    public GameObject[] reseeders = new GameObject[9];
    public GameObject[] resprouters = new GameObject[9];
   
    public struct data
    {
        public int month;
        public int year;
        public float biomass;
        public float precip;
        public float bushScale;

        public data(int m, int y, float b, float p, float s)
        {
            month = m;
            year = y;
            biomass = b;
            precip = p;
            bushScale = s;
        }
    }
    //gameObject.finf(Project Manager)
    //check for other PM in Awake functions


    //want to know the minumum and maximum biomass values in each data file
    //Syntax: O = Obligate Reseeder, R  = Resprouter (leading character in the following variables) 

    public float OMaxBioDry = 0;
    public float OMinBioDry = 0;
    public float OMaxBioWet = 0;
    public float OMinBioWet = 0;

    public float RMaxBioDry =  0;
    public float RMinBioDry = 0;
    public float RMaxBioWet = 0;
    public float RMinBioWet = 0;

    public List<data> ReseederDry = new List<data>();
    public List<data> ReseederWet = new List<data>();
    public List<data> ResprouterDry = new List<data>();
    public List<data> ResprouterWet = new List<data>();

    private void Awake()
    {
        Debug.Log("starting manager");
        DontDestroyOnLoad(this.gameObject);
        //check if other PM exists
        // go through array and delete other existing managers if any

        Read("Ceanothus_burn_dryP", ref ReseederDry, ref OMinBioDry, ref OMaxBioDry, "dry");
        Read("Ceanothus_burn_wetP", ref ReseederWet, ref OMinBioWet, ref OMaxBioWet, "wet");
        Read("Chamise_burn_dryP", ref ResprouterDry, ref RMinBioDry, ref RMaxBioDry, "dry");
        Read("Chamise_burn_wetP", ref ResprouterWet, ref RMinBioWet, ref RMaxBioWet, "wet");

    }

    //read from fileName.txt, and return a list of all values of targetVar in the file
    public void Read(string fileName, ref List<data> list, ref float min, ref float max, string climate)
    {

        string path = Application.dataPath + "/Data/" + fileName + ".txt";
        float tempMin;
        float tempMax;

        // Create an instance of StreamReader to read from a file.
        // The using statement also closes the StreamReader.
        using (StreamReader sr = new StreamReader(path))
        {
            string line = sr.ReadLine();    //read first line to find idx of target Variable
            string[] values = line.Split(' ');  //get all the names of the values that in the data file

            //need to find "day", "month", "biomass", and "precip"
            int dayIdx = Array.IndexOf(values, "day");
            int monthIdx = Array.IndexOf(values, "month");
            int yearIdx = Array.IndexOf(values, "year");
            int bioIdx = Array.IndexOf(values, "biomass");
            int precipIdx = Array.IndexOf(values, "precip");

            if (dayIdx < 0) { throw new System.ArgumentException("'day' data does not exist in file " + fileName); }
            if (monthIdx < 0) { throw new System.ArgumentException("'month' data does not exist in file " + fileName); }
            if (yearIdx < 0) { throw new System.ArgumentException("'year' data does not exist in file " + fileName); }
            if (bioIdx < 0) { throw new System.ArgumentException("'biomass' data does not exist in file " + fileName); }
            if (precipIdx < 0) { throw new System.ArgumentException("'precip' data does not exist in file " + fileName); }

            //get starting values
            line = sr.ReadLine();
            values = line.Split(' ');
            //int currDay = int.Parse(values[dayIdx]); // do i even need this
            int month = int.Parse(values[monthIdx]);
            int year = int.Parse(values[yearIdx]);
            float biomass = float.Parse(values[bioIdx]);
            float precip = float.Parse(values[precipIdx]);

            int originalYear = year;
            list.Add(new data(month, year, biomass, precip, 0));
            //data currDataObject = new data(currMonth, currYear, bioSum, precipSum);

            tempMin = biomass;
            tempMax = biomass;

            //find monthly averages of biomass and 
            while ((line = sr.ReadLine()) != null)
            {
                values = line.Split(' ');
                year = int.Parse(values[yearIdx]);


                if (year < originalYear + 15)
                {

                    biomass = float.Parse(values[bioIdx]);

                    if (biomass < tempMin)
                    {
                        tempMin = biomass;
                    }
                    else if (biomass > tempMax)
                    {
                        tempMax = biomass;
                    }

                    list.Add(new data(int.Parse(values[monthIdx]), year, biomass, float.Parse(values[precipIdx]), 0));
                }
                else
                {
                    //end while loop because we've already read 15 years of data post fire
                    break;
                } 

            }
        }

        scaleBiomass(ref list, ref tempMin, ref tempMax);
        min = tempMin;
        max = tempMax;

    }

    private void scaleBiomass(ref List<data> list, ref float min, ref float max)
    {
        float scale;
        float percentage;
        scale = (list[0].biomass * 2.25f) / max;
        list[0] = new data(list[0].month, list[0].year, list[0].biomass, list[0].precip, scale);

        min = scale;
        max = 0;
        for (int i = 1; i < list.Count; i++)
        {
            percentage = (list[i].biomass - list[i - 1].biomass) / list[i - 1].biomass;
            scale = list[i - 1].bushScale * (1 + percentage);

            if (scale < min)
            {
                min = scale;
            }
            if (scale > max)
            {
                max = scale;
            }

            list[i] = new data(list[i].month, list[i].year, list[i].biomass, list[0].precip, scale);
        }



    }


    private void Start()
    {
    }
}
