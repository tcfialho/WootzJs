﻿#region License

//-----------------------------------------------------------------------
// <copyright>
// The MIT License (MIT)
// 
// Copyright (c) 2014 Kirk S Woll
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

#endregion

using System;
using WootzJs.Testing;

namespace WootzJs.Compiler.Tests
{
    public class DateTimeTests : TestFixture
    {
        [Test]
        public void CreateDate()
        {
            var date = new DateTime(2014, 1, 2, 15, 3, 5, 30);
            AssertEquals(date.Year, 2014);
            AssertEquals(date.Month, 1);
            AssertEquals(date.Day, 2);
            AssertEquals(date.Hour, 15);
            AssertEquals(date.Minute, 3);
            AssertEquals(date.Second, 5);
            AssertEquals(date.Millisecond, 30);
        }

        [Test]
        public void AddMilliseconds()
        {
            var originalDate = new DateTime(2012, 1, 1, 0, 0, 0, 995);
            var newDate = originalDate.AddMilliseconds(10);
            
            AssertEquals(newDate.Second, 1);
            AssertEquals(newDate.Millisecond, 5);
        }

        [Test]
        public void AddSeconds()
        {
            var originalDate = new DateTime(2012, 1, 1, 0, 0, 55, 0);
            var newDate = originalDate.AddSeconds(10);
            
            AssertEquals(newDate.Minute, 1);
            AssertEquals(newDate.Second, 5);            
        }

        [Test]
        public void AddMinutes()
        {
            var originalDate = new DateTime(2012, 1, 1, 0, 55, 0, 0);
            var newDate = originalDate.AddMinutes(10);
            
            AssertEquals(newDate.Hour, 1);
            AssertEquals(newDate.Minute, 5);            
        }

        [Test]
        public void AddHours()
        {
            var originalDate = new DateTime(2012, 1, 1, 23, 0, 0, 0);
            var newDate = originalDate.AddHours(2);
            
            AssertEquals(newDate.Hour, 1);
            AssertEquals(newDate.Day, 2);            
        }

        [Test]
        public void AddDays()
        {
            var originalDate = new DateTime(2012, 1, 31, 0, 0, 0, 0);
            var newDate = originalDate.AddDays(1);
            
            AssertEquals(newDate.Day, 1);
            AssertEquals(newDate.Month, 2);            
        }

        [Test]
        public void AddMonths()
        {
            var originalDate = new DateTime(2012, 12, 1, 0, 0, 0, 0);
            var newDate = originalDate.AddMonths(1);
            
            AssertEquals(newDate.Month, 1);
            AssertEquals(newDate.Year, 2013);                        
        }

        [Test]
        public void AddYears()
        {
            var originalDate = new DateTime(2012, 1, 1, 0, 0, 0, 0);
            var newDate = originalDate.AddYears(1);
            
            AssertEquals(newDate.Year, 2013);
        }

        [Test]
        public void SingleDigitMonth()
        {
            var time = new DateTime(2014, 1, 1);
            var s = time.ToString("M");
            AssertEquals(s, "1");
        }

        [Test]
        public void SingleDigitMonthWithTwoDigits()
        {
            var time = new DateTime(2014, 12, 1);
            var s = time.ToString("M");
            AssertEquals(s, "12");
        }

        [Test]
        public void TwoDigitMonth()
        {
            var time = new DateTime(2014, 1, 1);
            var s = time.ToString("MM");
            AssertEquals(s, "01");
        }

        [Test]
        public void TwoDigitMonthWithTwoDigits()
        {
            var time = new DateTime(2014, 12, 1);
            var s = time.ToString("MM");
            AssertEquals(s, "12");
        }

        [Test]
        public void SingleDigitDay()
        {
            var time = new DateTime(2014, 12, 3);
            var s = time.ToString("d");
            AssertEquals(s, "3");
        }

        [Test]
        public void SingleDigitDayTwoDigit()
        {
            var time = new DateTime(2014, 12, 29);
            var s = time.ToString("d");
            AssertEquals(s, "29");
        }

        [Test]
        public void TwoDigitDay()
        {
            var time = new DateTime(2014, 12, 3);
            var s = time.ToString("dd");
            AssertEquals(s, "03");
        }

        [Test]
        public void TwoDigitDayTwoDigit()
        {
            var time = new DateTime(2014, 12, 29);
            var s = time.ToString("dd");
            AssertEquals(s, "29");
        }

        [Test]
        public void SingleDigitYear()
        {
            var time = new DateTime(2003, 12, 3);
            var s = time.ToString("y");
            AssertEquals(s, "3");
        }

        [Test]
        public void SingleDigitYearTwoDigit()
        {
            var time = new DateTime(2014, 12, 29);
            var s = time.ToString("y");
            AssertEquals(s, "14");
        }

        [Test]
        public void TwoDigitYear()
        {
            var time = new DateTime(2003, 12, 3);
            var s = time.ToString("yy");
            AssertEquals(s, "03");
        }

        [Test]
        public void TwoDigitYearTwoDigit()
        {
            var time = new DateTime(2014, 12, 29);
            var s = time.ToString("yy");
            AssertEquals(s, "14");
        }

        [Test]
        public void FourDigitYear()
        {
            var time = new DateTime(2003, 12, 3);
            var s = time.ToString("yyyy");
            AssertEquals(s, "2003");
        }

        [Test]
        public void SingleDigitHour()
        {
            var time = new DateTime(2014, 12, 3, 17, 3, 10);
            var s = time.ToString("h");
            AssertEquals(s, "5");
        }

        [Test]
        public void SingleDigitHourTwoDigit()
        {
            var time = new DateTime(2014, 12, 3, 22, 3, 10);
            var s = time.ToString("h");
            AssertEquals(s, "10");
        }

        [Test]
        public void TwoDigitHour()
        {
            var time = new DateTime(2014, 12, 3, 17, 3, 10);
            var s = time.ToString("hh");
            AssertEquals(s, "05");
        }

        [Test]
        public void TwoDigitHourTwoDigit()
        {
            var time = new DateTime(2014, 12, 3, 22, 3, 10);
            var s = time.ToString("hh");
            AssertEquals(s, "10");
        }

        [Test]
        public void SingleDigitMinute()
        {
            var time = new DateTime(2014, 12, 3, 17, 3, 10);
            var s = time.ToString("m");
            AssertEquals(s, "3");
        }

        [Test]
        public void SingleDigitMinuteTwoDigit()
        {
            var time = new DateTime(2014, 12, 3, 22, 13, 10);
            var s = time.ToString("m");
            AssertEquals(s, "13");
        }

        [Test]
        public void TwoDigitMinute()
        {
            var time = new DateTime(2014, 12, 3, 17, 3, 10);
            var s = time.ToString("mm");
            AssertEquals(s, "03");
        }

        [Test]
        public void TwoDigitMinuteTwoDigit()
        {
            var time = new DateTime(2014, 12, 3, 22, 13, 10);
            var s = time.ToString("mm");
            AssertEquals(s, "13");
        }
    }
}