using System;
using NLog;
using TestNLog.Company.Product.BusinessLogic;
using TestNLog.Company.Product.ServiceTester;
namespace TestNLog
{
  class Program
  {
    static readonly Logger _log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            ////if (!EventLog.SourceExists("log4net"))
            ////{
            ////    //An event log source should not be created and immediately used.
            ////    //There is a latency time to enable the source, it should be created
            ////    //prior to executing the application that uses the source.
            ////    //Execute this sample a second time to use the new source.
            ////    EventLog.CreateEventSource("log4net", "Application");
            ////}

            ConsoleKeyInfo key = new ConsoleKeyInfo();
            while (key.Key != ConsoleKey.X)
            {

                DoLog(key.KeyChar);


                DoWinDebug();
                key = Console.ReadKey();
            }
        }

        static void DoWinDebug()
        {
            //Console.WriteLine("Begin Doing WinDebug!");
            //System.Diagnostics.Debug.WriteLine("This is a call to System.Diagnostics.Debug");
            //System.Diagnostics.Trace.WriteLine("This is a call to System.Diagnostics.Trace");
            //Console.WriteLine("End Doing WinDebug!");
        }

        static void DoLog(char keyChar)
        {
            Console.WriteLine("\nBegin Doing Log!");
            Console.WriteLine(DateTime.Now);
            if (Char.ToLower(keyChar) == 'b')
            {
                for (int i = 0; i < 10000; i++)
                {
                    _log.Info(i);
                }
            }
            else if (keyChar >= '1' && keyChar <= '9')
            {
                _log.Info(keyChar);
            }
            else if (Char.ToLower(keyChar) == 'e')
            {
                try
                {
                    ThrowTestEx();
                    return;
                }
                catch (Exception ex)
                {
                    _log.Error("TestEx", ex);
                }
            }
            else if (Char.ToLower(keyChar) == 'c')
            {
                for (int i = 0; i < 10; i++)
                {
                    _log.Info("测试中文");
                }
            }
            else
            {
                if (_log.IsErrorEnabled)
                    _log.Error("This is an Error...");

                if (_log.IsDebugEnabled)
                    for (int i = 0; i < 10; i++)
                        _log.Debug("This is a simple log!");

                if (_log.IsErrorEnabled)
                    _log.Error("This is an Error...");

                if (_log.IsInfoEnabled)
                    _log.Info("This is an Info...");


                _log.Warn("This is a Warning...");
                _log.Fatal("This is a Fatal...");

                _log.Error("This is an error with an exception.", new Exception("The message exception here."));

                _log.Warn("This is a message on many lines...\nlines...\nlines...\nlines...");
                _log.Warn("This is a message on many lines...\r\nlines...\r\nlines...\r\nlines...");

                DummyManager dm = new DummyManager();
                dm.DoIt();

                DummyTester dt = new DummyTester();
                dt.DoIt();
            }

            Console.WriteLine("End Doing Log!");
        }

        private static void ThrowTestEx()
        {
            throw new Exception("TestEx");
        }

    }

  namespace Company.Product.BusinessLogic
  {

    public class DummyManager
    {
      public static readonly Logger Log = LogManager.GetCurrentClassLogger();


      public DummyManager()
      {
        if (Log.IsInfoEnabled)
          Log.Info("Dummy Manager ctor");
      }

      public void DoIt()
      {
        if (Log.IsDebugEnabled)
          Log.Debug("DM: Do It, Do It Now!!");

        Log.Warn("This is a Warning from DM...");
        Log.Error("This is an Error from DM...");
        Log.Fatal("This is a Fatal from DM...");

        Log.ErrorException("This is an error from DM with an exception.", new Exception("The message exception here."));
      }
    }
  }

  namespace Company.Product.ServiceTester
  {

    public class DummyTester
    {
      public static readonly Logger Log = LogManager.GetCurrentClassLogger();

      public DummyTester()
      {
        if (Log.IsInfoEnabled)
          Log.Info("Dummy Tester ctor");
      }

      public void DoIt()
      {
        if (Log.IsDebugEnabled)
          Log.Debug("DT: Do It, Do It Now!!");

        Log.Warn("This is a Warning from DT...");
        Log.Error("This is an Error from DT...");
        Log.Fatal("This is a Fatal from DT...");

        Log.ErrorException("This is an error from DT with an exception.", new Exception("The message exception here."));
      }
    }
  }
}
