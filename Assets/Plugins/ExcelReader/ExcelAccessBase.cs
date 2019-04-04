using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel;
using System.Data;
using System.IO;

public abstract   class ExcelAccessBase<T> where T :struct  {


    public  abstract  List<T> CreateList();

    /// <summary>
    /// 读取Excel表
    /// </summary>
    /// <param name="excelName">在Assets下的全路径</param>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    protected static DataRowCollection ReadExcel(string excelName, string sheetName)
    {
        string path = Application.dataPath + "/" + excelName;
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = reader.AsDataSet();
        return result.Tables[sheetName].Rows;
    }

}
