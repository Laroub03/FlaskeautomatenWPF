using Flaskeautomaten_WPF;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flaskautomaten
{
    public class Program
    {
        private const int maxBoxSize = 10;

        // Create queues to hold items produced by the Producer and split by the Splitter
        static Queue<string> _beerBox = new Queue<string>();
        static Queue<string> _sodaBox = new Queue<string>();
        static Queue<string> _itemBox = new Queue<string>();

        public async Task StartSimulation(Action<string> outputCallback)
        {
            Program program = new Program();
            // Create a producer, a splitter, and two consumer threads
            Thread producerThread = new Thread(new ThreadStart(() => program.Producer(outputCallback)));
            Thread splitterThread = new Thread(new ThreadStart(() => program.Splitter(outputCallback)));
            Thread beerConsumerThread = new Thread(new ThreadStart(() => program.BeerConsumer(outputCallback)));
            Thread sodaConsumerThread = new Thread(new ThreadStart(() => program.SodaConsumer(outputCallback)));

            producerThread.Start();
            splitterThread.Start();
            beerConsumerThread.Start();
            sodaConsumerThread.Start();

            producerThread.Join();
            splitterThread.Join();
            beerConsumerThread.Join();
            sodaConsumerThread.Join();
        }
    
        // A method to produce items
        public void Producer(Action<string> outputCallback)
        {
            int counter = 1;
            while (true)
            {
                try
                {
                    lock (_itemBox)
                    {
                        // Produce a beer or soda item and add it to the queue
                        string item = (counter % 2 == 0) ? "øl" : "sodavand";
                        item += counter.ToString();
                        _itemBox.Enqueue(item);
                        outputCallback($"Producer har produceret: {item}");
                        counter++;

                        // Random delay
                        Random rnd = new Random();
                        int number = rnd.Next(1, 2000);
                        Thread.Sleep(number);

                        // Signal the waiting threads that there are items in the queue
                        if (_itemBox.Count > 0)
                        {
                            outputCallback("Producer waits ");
                            Monitor.PulseAll(_itemBox);
                        }
                    }
                }
                catch (Exception e)
                {
                    // handle any errors that occur
                    outputCallback($"Error caught: {e}");
                }
            }
        }

        // A method to split items into two queues
        public void Splitter(Action<string> outputCallback)
        {
            while (true)
            {
                try
                {
                    lock (_itemBox)
                    {
                        // If the _itemBox is not empty, split the items into two queues
                        if (_itemBox.Count > 0)
                        {
                            string item = _itemBox.Dequeue();

                            // Check if the item is a beer or soda, and enqueue it in the respective queue
                            if (item.StartsWith("øl"))
                            {
                                try
                                {
                                    lock (_beerBox)
                                    {
                                        // If the beer buffer is full, wait until there are spaces in the queue
                                        if (_beerBox.Count == maxBoxSize)
                                        {
                                            Monitor.Wait(_beerBox);
                                        }
                                        _beerBox.Enqueue(item);
                                        outputCallback($"Splitter har sendt en flaske øl til BeerConsumer");

                                        // Signal the waiting threads that there are items in the queue
                                        if (_beerBox.Count > 0)
                                        {
                                            outputCallback("Splitter waits ");
                                            Monitor.PulseAll(_beerBox);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    // handle any errors that occur
                                    outputCallback($"Error caught: {e}");
                                }
                            }
                            else
                            {
                                try
                                {
                                    lock (_sodaBox)
                                    {
                                        // If the soda buffer is full, wait until there are spaces in the queue
                                        if (_sodaBox.Count == maxBoxSize)
                                        {
                                            Monitor.Wait(_sodaBox);
                                        }
                                        _sodaBox.Enqueue(item);
                                        outputCallback($"Splitter har sendt en flaske sodavand til SodaConsumer");

                                        // Signal the waiting threads that there are items in the queue
                                        if (_sodaBox.Count > 0)
                                        {
                                            outputCallback("Splitter waits ");
                                            Monitor.PulseAll(_sodaBox);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    // handle any errors that occur
                                    outputCallback($"Error caught: {e}");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // handle any errors that occur
                    outputCallback($"Error caught: {e}");
                }
            }
        }

        // A method to consume soda items
        public void SodaConsumer(Action<string> outputCallback)
        {
            while (true)
            {
                try
                {
                    lock (_sodaBox)
                    {
                        // If the soda buffer is empty, wait until there are items in the queue
                        if (_sodaBox.Count == 0)
                        {
                            Monitor.Wait(_sodaBox);
                        }
                        // Remove a soda item from the queue and consume it
                        string item = _sodaBox.Dequeue();
                        outputCallback($"SodaConsumer har consumeret: {item}");

                        // If the soda buffer is empty, signal the waiting threads
                        if (_sodaBox.Count == 0)
                        {
                            outputCallback("SodaConsumer waits ");
                        }

                        // Random delay
                        Random rnd = new Random();
                        int number = rnd.Next(1, 2000);
                        Thread.Sleep(number);

                        // Signal the waiting threads that there are spaces in the queue
                        if (_sodaBox.Count < maxBoxSize)
                        {
                            outputCallback("SodaConsumer waits ");
                            Monitor.PulseAll(_sodaBox);
                        }
                    }
                }
                catch (Exception e)
                {
                    // handle any errors that occur
                    outputCallback($"Error caught: {e}");
                }
            }
        }

        // A method to consume beer items
        public void BeerConsumer(Action<string> outputCallback)
        {
            while (true)
            {
                try
                {
                    lock (_beerBox)
                    {
                        // If the beer buffer is empty, wait until there are items in the queue
                        if (_beerBox.Count == 0)
                        {
                            Monitor.Wait(_beerBox);
                        }
                        // Remove a beer item from the queue and consume it
                        string item = _beerBox.Dequeue();
                        outputCallback($"BeerConsumer har consumeret: {item}");

                        // If the beer buffer is empty, signal the waiting threads
                        if (_beerBox.Count == 0)
                        {
                            outputCallback("BeerConsumer waits ");
                        }

                        // Random delay
                        Random rnd = new Random();
                        int number = rnd.Next(1, 2000);
                        Thread.Sleep(number);

                        // Signal the waiting threads that there are spaces in the queue
                        if (_beerBox.Count < maxBoxSize)
                        {
                            outputCallback("BeerConsumer waits ");
                            Monitor.PulseAll(_beerBox);
                        }
                    }
                }
                catch (Exception e)
                {
                    // handle any errors that occur
                    outputCallback($"Error caught: {e}");
                }
            }
        }
    }
}