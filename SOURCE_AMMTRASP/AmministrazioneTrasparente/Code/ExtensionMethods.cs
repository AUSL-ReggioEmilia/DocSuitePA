using System;
using Telerik.Web.UI;

namespace AmministrazioneTrasparente
{
    public static class ExtensionMethods
    {
        /// <summary> Data selezionata o scritta nel campo di testo </summary>
        /// <remarks> Pezza momentanea per gestire ricerche eseguite dopo un postback </remarks>
        public static DateTime? UnsafeSelectedDate(this RadDatePicker datePicker)
        {
            if (datePicker.SelectedDate.HasValue)
                return datePicker.SelectedDate;
            DateTime unsafeDateTime;
            if (DateTime.TryParse(datePicker.InvalidTextBoxValue, out unsafeDateTime))
                return unsafeDateTime;
            return null;
        }
    }
}