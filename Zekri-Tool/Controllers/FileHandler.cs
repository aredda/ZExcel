using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zekri_Tool.Models;
using Zekri_Tool.Models.Interfaces;

namespace Zekri_Tool.Controllers
{
    public class FileHandler
    {
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private List<string> columns;
        public List<string> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private List<IParsable> rows;
        public List<IParsable> Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        public FileHandler()
        {}
        public FileHandler(string fileName, List<string> cols)
            : this ()
        {
            this.FileName = fileName;
            this.Columns = cols;
        }
        public FileHandler (string fileName, List<IParsable> rows)
            : this (fileName, rows.First().GetColumns())
        {
            this.Rows = rows;
        }

        public List<Range> GetRanges(Worksheet sheet, bool setUp = false)
        {
            List<Range> ranges = new List<Range>();

            // create ranges if a setup is needed
            if (setUp)
                for (int x = 0; x < Columns.Count; x++)
                    sheet.Cells[1, x + 1].Value = Columns[x];
            
            // find ranges
            foreach (string column in Columns) ranges.Add(sheet.Cells.Find(column));

            return ranges;
        }

        public void Save()
        {
            // indicator
            bool isCreated = File.Exists(this.fileName);
            // open app
            Application app = new Application();
            Workbook book = !isCreated ? app.Workbooks.Add() : app.Workbooks.Open(this.fileName);
            Worksheet sheet = book.ActiveSheet;
            // clear sheet
            sheet.Cells.ClearContents();
            // find ranges
            List<Range> ranges = GetRanges (sheet, true);
            // write data
            int xi = ranges.First().Column, yi = ranges.First().Row + 1;
            for (int y = 0; y < Rows.Count; y++)
                for (int x = 0; x < Rows[y].Parse().Count; x++)
                    sheet.Cells[yi + y, xi + x - 1].Value = Rows[y].Parse()[x];
            // save changes
            if (isCreated) book.Save();
            else book.SaveAs(this.fileName);
            // close app
            app.Quit();
        }

        public void Show()
        {
            // open app
            Application app = new Application();
            Workbook book = app.Workbooks.Open(this.fileName);
            Worksheet sheet = book.ActiveSheet;
            // show
            app.Visible = true;
        }

        public List<List<string>> Read()
        {
            // initialize empty container
            List<List<string>> data = new List<List<string>>();
            // open app
            Application app = new Application();
            Workbook book = app.Workbooks.Open(this.fileName);
            Worksheet sheet = book.ActiveSheet;
            // get ranges
            List<Range> ranges = GetRanges(sheet);
            int xi = ranges.First().Column, yi = ranges.First().Row + 1, y = 0;
            bool eof = false;
            // fetch document for data
            while (!eof)
            {
                // container for a single row
                List<string> row = new List<string>();
                // fetch
                for (int x=0; x<Columns.Count && !eof; x++)
                {
                    object value = sheet.Cells[yi + y, xi + x - 1].Value;

                    Debug.WriteLine(value);

                    if (x == 0 && value == null)
                        eof = true;

                    row.Add(value == null ? "" : value.ToString());
                }
                // add row
                if (!eof) data.Add(row);
                // move to the next row
                y++;
            }
            // close
            app.Quit();
            // return
            return data;
        }
    }
}
