#region License
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
using System.Runtime.WootzJs;
using WootzJs.Testing;

namespace WootzJs.Compiler.Tests
{
    public class TryTests : TestFixture
    {
        [Test]
        public void ExceptionCaught()
        {
            try
            {
                throw new Exception();
                AssertTrue(false);
            }
            catch (Exception e)
            {
                AssertTrue(true);
            }
        }
         
        [Test]
        public void FinallyExecuted()
        {
            bool success = false;
            try
            {
                try
                {
                    throw new Exception();
                }
                finally
                {
                    success = true;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception e)
            {
            }
            AssertTrue(success);
        }
         
        [Test]
        public void NakedCatchClause()
        {
            try
            {
                throw new Exception();
                AssertTrue(false);
            }
            catch 
            {
                AssertTrue(true);
            }
        }

        [Test] 
        public void MultipleCatchClauses()
        {
            try
            {
                throw new InvalidOperationException();
            }
            catch (InvalidOperationException e)
            {
                AssertEquals(e.GetType().Name, "InvalidOperationException");
            }
            catch (Exception e)
            {
                AssertTrue(false);
            }
        }

        [Test]
        public void DontCatchWrongException()
        {
            var flag = true;
            try
            {
                try
                {
                    throw new InvalidOperationException();
                }
                catch (TaskCanceledException)
                {
                    flag = false;
                }                
            }
            catch (Exception)
            {
            }
            AssertTrue(flag);
        }

        [Test]
        public void WrongExceptionBubbles()
        {
            var flag = false;
            try
            {
                try
                {
                    throw new InvalidOperationException();
                }
                catch (TaskCanceledException)
                {
                }
            }
            catch (InvalidOperationException)
            {
                flag = true;
            }
            AssertTrue(flag);
        }
    }
}
