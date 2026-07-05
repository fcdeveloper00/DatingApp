namespace API.Extensions;

public static class DateTimeExtensions
{

    public static int CalculateAge(this DateOnly dob)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        int age = today.Year - dob.Year;

        if (dob > today.AddYears(-age))
            age--;
        return age;

        #region MyRegion

        //DateTime today = DateTime.Now.Date;

        /*
         if(today.Month == dob.Month)
        {
            if(today.Day == dob.Day || today.Day > dob.Day)
            {
                return today.Year - dob.Year;
            }
            else
            {
                return (today.Year - dob.Year) - 1;
            }
        }
        else
        {
            if(today.Month > dob.Month)
            {
                return today.Year - dob.Year;
            }
            else
            {
                return (today.Year - dob.Year) -1;
            }
        }
        */

        #endregion
    }

}
