using System;
using System.Collections.Generic;

namespace ClassAnalyzer
{
    public interface IClassAnalyzer
    {
        string GetStringObjRepresentation<T>(T obj);
        string GetStringObjRepresentations<T>(IEnumerable<T> collection, int limitOutput = 25);
    }
}