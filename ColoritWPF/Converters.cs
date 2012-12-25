﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ColoritWPF
{
    //Radio buttons converter   
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class ServiceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from service in сolorItEntities.ServiceTypes
                                where service.ID == (int)value
                                select service.ServiceName).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            return "Сервис не найден";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CarModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from car in сolorItEntities.CarModels
                                where car.ID == (int)value
                                select car.ModelName).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
                return "Модель не найдена";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PaintNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from paint in сolorItEntities.PaintName
                                where paint.ID == (int)value
                                select paint.Name).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            return "Наименование краски не найдено";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsL2KConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from paint in сolorItEntities.PaintName
                                where paint.ID == (int) value
                                select paint.L2K).First();

                    return bool.Parse(grId.ToString());
                }
            }
            throw new Exception("Не вышло достать значение L2K для радио кнопки");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from paint in сolorItEntities.PaintName
                                where paint.ID == (int) value
                                select paint.L2K).First();

                    return bool.Parse(grId.ToString());
                }
            }
            throw new Exception("Не вышло достать значение L2K для радио кнопки");
        }
    }

    public class ProductGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                if (value != null)
                {
                    var grList = CIentity.Group.FirstOrDefault(c => c.ID == (int)value);
                    return ((Group)grList).Name.ToString();
                }
                else
                    return "Без группы";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Этот конвертер только для группировки");
        }
    }

    public class ClientNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from client in сolorItEntities.Client
                                where client.ID == (int)value
                                select client.Name).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
                return "Клиент не найден";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
