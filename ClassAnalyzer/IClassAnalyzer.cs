using System;

namespace ClassAnalyzer
{
    public interface IClassAnalyzer
    {
        string GetStringObjRepresentation<TBaseType>(TBaseType obj);
    }
}