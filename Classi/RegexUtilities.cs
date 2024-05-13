using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MicriCancelli.Classi
{
    public class RegexUtilities
    {
        //utilizzato per validare il formato della mail
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool IsValidCAP(string cap)
        {
            try
            {
                return Regex.IsMatch(cap, @"^(V-|I-)?[0-9]{5}$");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool IsValidProvincia(string provincia)
        {
            try
            {
                return Regex.IsMatch(provincia, @"^(V-|I-)?[A-Z]{2}$");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool IsValidData(string data)
        {
            try
            {
                return Regex.IsMatch(data, @"^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool IsValidCF(string data)
        {
            try
            {

                //return Regex.IsMatch(data, @"^[a-zA-Z]{6}[0-9]{2}[abcdehlmprstABCDEHLMPRST]{ 1}[0-9]{ 2} ([a - zA - Z]{ 1}[0-9]{ 3})[a-zA-Z]{ 1}$");
                return Regex.IsMatch(data, @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

        }
        public static bool IsValidCF_2(string data)
        {
            try
            {
                return Regex.IsMatch(data, @"/ ^(?:[A - Z][AEIOU][AEIOUX] |[AEIOU]X{ 2}|[B - DF - HJ - NP - TV - Z]{ 2}
                                             [A-Z]){ 2} (?:[\dLMNP - V]{ 2} (?:[A - EHLMPR - T](?:[04LQ][1 - 9MNP - V] 
                                             |[15MR][\dLMNP - V] |[26NS][0 - 8LMNP - U]) |[DHPS][37PT][0L] |[ACELMRT][37PT][01LM]
                                             |[AC - EHLMPR - T][26NS][9V])| (?:[02468LNQSU][048LQU] |[13579MPRTV][26NS])B[26NS][9V])
                                             (?:[A - MZ][1 - 9MNP - V][\dLMNP - V]{ 2}|[A - M][0L](?:[1 - 9MNP - V][\dLMNP - V] 
                                             |[0L][1 - 9MNP - V]))[A-Z]$/ i");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool IsValidCell(string cell)
        {
            try
            {
                return Regex.IsMatch(cell, @"^3\d{8,9}$");
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
