

//Using statments
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GradeTracker
{
    class GradeTracker
    {
        // Declare constants for the file path names 
        private const string LIST_FILE = "grades.json";
        private const string SCHEMA_FILE = "grade-schema1.json";
        static void Main(string[] args)
        {
            //Global variables
            string json_schema;
            string jsondata;
            bool done = false;
            List<Item> Courses = new List<Item>();
            List<Item> CourseGoal = new List<Item>();
            List<double> allGPAs = new List<double>();
            CourseHolder holder = new CourseHolder();
            AskUser();
            //Reading schema file
            if (ReadFile(SCHEMA_FILE, out json_schema))
            {
                //Reading json file
                if (ReadFile(LIST_FILE, out jsondata))
                {
                    //Making json file into an object to use
                    holder = JsonConvert.DeserializeObject<CourseHolder>(jsondata);
                    //Taking list out of the courseholder
                    Courses = holder.courses;
                    CourseGoal = holder.courses;
                    //Validating input
                    for (int i = 0; i < Courses.Count; i++)
                    {
                        if (!ValidateItem(Courses[i], json_schema))
                        {
                            Console.WriteLine("Grades data file grades.json does not comply with json_schema.\nPlease modify your json file");
                            Environment.Exit(1);
                        }
                    }
                    for (int i = 0; i < CourseGoal.Count; i++)
                    {
                        if (!ValidateItem(CourseGoal[i], json_schema))
                        {
                            Console.WriteLine("Grades data file grades.json does not comply with json_schema.\nPlease modify your json file");
                            Environment.Exit(1);
                        }
                    }
                }
                else
                {
                    //Giving user the ability to make own json file
                    Console.Write("Grades data file grades.json not found. Create new file? (Y/N):");
                    object input = Console.ReadLine();
                    if (input.Equals("Y"))
                    {
                        Console.Write("\nNew Data set created. Press any key to continue...");
                        Console.ReadLine();
                    }
                    //Closing program if no is selected
                    else if (input.Equals("N"))
                    {
                        Console.WriteLine("\nThanks for running my program, have a wonderful day");
                        Environment.Exit(1);
                    }
                }
                do
                {
                    //Creating variables for needed dor do while loop
                    Console.Clear();
                    bool valid;
                    Item item = new Item();
                    //Printing the title
                    PrintTitle();
                    //Checking to see if courses need to be printed out
                    if (Courses.Count == 0)
                    {
                        Console.WriteLine("There are currently no saved courses.");
                    }
                    else
                    {
                        //Printing out titles
                        Console.Write("#. Course\t");
                        Console.Write("Marks Earned\t");
                        Console.Write("Out Of\t");
                        Console.WriteLine("Percent\n");
                        for (int i = 0; i < Courses.Count; i++)
                        {
                            Console.Write(i + 1 + ". ");
                            Console.Write(Courses[i].Code + "\t\t");
                            //Checking to see if evaluations in null
                            if (Courses[i].Evalulations == null)
                            {
                                Console.Write("0.0\t");
                                Console.Write("0.0\t");
                                Console.WriteLine("0.0");
                            }
                            else
                            {
                                //Preparing for calculations
                                double totalEarned;
                                double totalPotential;
                                double?[] tempEarned = new double?[Courses[i].Evalulations.Count];
                                double[] tempPotential = new double[Courses[i].Evalulations.Count];
                                //Gathering all evaluations to do calculations
                                for (int j = 0; j < Courses[i].Evalulations.Count; j++)
                                {
                                    tempEarned[j] = Courses[i].Evalulations[j].EarnedMarks;
                                    tempPotential[j] = Courses[i].Evalulations[j].OutOf;
                                    allGPAs.Add(CalcGPAforClass(Courses[i]));
                                }
                                //perfomring and printing calulations
                                totalEarned = CalcCourseTotalEarned(tempEarned);
                                totalPotential = CalcCourseTotalPotential(tempPotential);
                                Console.Write(String.Format("{0:0.0}", totalEarned) + "\t");
                                Console.Write(String.Format("{0:0.0}", totalPotential) + "\t");
                                Console.WriteLine(String.Format("{0:0.0}", CalcCoursePercent(totalEarned, totalPotential)));
                            }
                        }
                        Console.Write("\nTotal GPA: ");
                        Console.Write(String.Format("{0:0.0}", CalcGPAforAllClasses(allGPAs.ToArray())));
                    }
                    //Printing info for first screen
                    PrintInfo();
                    object input = Console.ReadLine();
                    int tempInput = 0;
                    //Checking the type of input
                    if (int.TryParse((string)input, out tempInput))
                    {
                        //Checking if number is valid
                        if (Courses.Count > 0)
                        {
                            if (tempInput > 0 && tempInput <= Courses.Count)
                            {
                                //Going to the second screen
                                bool done2 = false;
                                do
                                {
                                    Evaluation evaluation = new Evaluation();
                                    bool valid2;
                                    //Printing title
                                    Console.Clear();
                                    PrintTitle();
                                    //Checking if there is evaluations
                                    if (Courses[tempInput - 1].Evalulations == null)
                                    {
                                        Console.WriteLine("There are currently no evalutions for " + Courses[tempInput - 1].Code);
                                        PrintInfo2();
                                        object input2 = Console.ReadLine();
                                        int tempInput2 = 0;
                                        //Checking second input to see if number
                                        if (int.TryParse((string)input2, out tempInput2))
                                        {
                                            //Checking if number is valid
                                            if (Courses[tempInput - 1].Evalulations.Count > 0)
                                            {
                                                bool done3 = false;
                                                bool valid3;
                                                if (tempInput2 > 0 && tempInput2 <= Courses[tempInput - 1].Evalulations.Count)
                                                {
                                                    //Going to the third screen
                                                    do
                                                    {
                                                        //Printing title
                                                        Console.Clear();
                                                        PrintTitle();
                                                        Console.Write("Marks Earned\t");
                                                        Console.Write("Out Of\t");
                                                        Console.Write("Percent\t");
                                                        Console.Write("Course Marks\t");
                                                        Console.Write("Weight/100\t");
                                                        Console.WriteLine("GPA\n");
                                                        //Checking if there is earned marks
                                                        if (Courses[tempInput - 1].Evalulations[tempInput2 - 1].EarnedMarks == null)
                                                        {
                                                            Console.Write("\t");
                                                            Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf) + "\t");
                                                            Console.Write("\t");
                                                        }
                                                        else
                                                        {

                                                            Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput2 - 1].EarnedMarks) + "\t\t");
                                                            Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf) + "\t\t");
                                                            Console.Write(String.Format("{0:0.0}", CalcCoursePercent(Courses[tempInput - 1].Evalulations[tempInput2 - 1].EarnedMarks, Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf)) + "\t\t");
                                                            Console.Write(String.Format("{0:0.0}", CalcCourseMarks(CalcCoursePercent(Courses[tempInput - 1].Evalulations[tempInput2 - 1].EarnedMarks, Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf), Courses[tempInput - 1].Evalulations[tempInput2 - 1].Weight)) + "\t\t");
                                                            //Console.Write(String.Format("{0:0.0}", CalcGPAforClass(Courses[tempInput -1])));
                                                        }
                                                        Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput2 - 1].Weight) + "\n");
                                                        PrintInfo3();
                                                        object input3 = Console.ReadLine();
                                                        //Checking input, no need to check type since only letters
                                                        //Delete
                                                        if (input3.Equals("D"))
                                                        {
                                                            //Checking if sure
                                                            Console.Write("Delete " + Courses[tempInput - 1].Evalulations[tempInput2 - 1].Description + "? (Y/N):");
                                                            string data = Console.ReadLine();
                                                            if (data.Equals("Y"))
                                                            {
                                                                //Deleting evaluations
                                                                if (Courses[tempInput - 1].Evalulations.Count > 1)
                                                                {
                                                                    Courses[tempInput - 1].Evalulations.RemoveAt(tempInput2 - 1);

                                                                }
                                                                //Deleting evalation list
                                                                else
                                                                {
                                                                    Courses[tempInput - 1].Evalulations = null;
                                                                }
                                                                done3 = true;
                                                            }
                                                        }
                                                        //Edit
                                                        else if (input3.Equals("E"))
                                                        {
                                                            do
                                                            {
                                                                //Asking for input
                                                                Console.Write("Enter the marks earned out of " + Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf + ", press ENTER to leave unassigned:");
                                                                string data = Console.ReadLine();
                                                                //Validating input
                                                                if (data == "")
                                                                {
                                                                    evaluation.EarnedMarks = null;
                                                                    valid3 = true;
                                                                }
                                                                else
                                                                {
                                                                    double temp;
                                                                    valid3 = double.TryParse(data, out temp);
                                                                    if (temp < 0 || temp >Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf)
                                                                    {
                                                                        valid3 = false;
                                                                    }
                                                                    if (valid3)
                                                                        evaluation.EarnedMarks = temp;
                                                                    else
                                                                        Console.WriteLine("\tERROR: 'Marks earned' must be a postive number that is less than "+ Courses[tempInput - 1].Evalulations[tempInput2 - 1].OutOf + " or null");
                                                                }
                                                            } while (!valid3);//End of validation loop
                                                            //Editing Earned marks and valiadting object
                                                            Courses[tempInput - 1].Evalulations[tempInput2 - 1].EarnedMarks = evaluation.EarnedMarks;
                                                            valid3 = ValidateItem(Courses[tempInput - 1], json_schema);
                                                            if (!valid3)
                                                                Console.WriteLine("\nERROR:\tItem data does not match the required format. Please try again.\n");


                                                        }
                                                        //Cancel
                                                        else if (input3.Equals("X"))
                                                        {
                                                            done3 = true;
                                                        }
                                                    } while (!done3);//End of third screen loop
                                                }
                                                //Validation of evaluation number
                                                else
                                                {
                                                    Console.WriteLine("\nERROR:\tThat course doesn't exist, please input a new number.\n");
                                                }
                                            }
                                            //Validation of course number
                                            else
                                            {
                                                Console.WriteLine("\nERROR:\tNo courses exist, please input a new course.\n");
                                            }
                                        }
                                        //Checking second input to see if string
                                        else if (input2.GetType() == typeof(string))
                                        {
                                            //Delete
                                            if (input2.Equals("D"))
                                            {
                                                //Checking if sure
                                                Console.Write("Delete " + Courses[tempInput - 1].Code + "? (Y/N):");
                                                string data = Console.ReadLine();
                                                //Deleting courses
                                                if (data.Equals("Y"))
                                                {
                                                    Courses.RemoveAt(tempInput - 1);
                                                    done2 = true;
                                                }
                                            }
                                            //Add
                                            else if (input2.Equals("A"))
                                            {
                                                do
                                                {
                                                    //Having user add evalutions while validating everything
                                                    Console.Write("Enter a description:");
                                                    evaluation.Description = Console.ReadLine();
                                                    do
                                                    {
                                                        Console.Write("Enter the 'out of' mark:");
                                                        string data = Console.ReadLine();
                                                        int temp;
                                                        valid2 = int.TryParse(data, out temp);
                                                        if (temp < 0) {
                                                            valid2 = false;
                                                        }
                                                        if (valid2)
                                                            evaluation.OutOf = temp;
                                                        else
                                                            Console.WriteLine("\tERROR: 'Out of' must be a postive integer number");
                                                    } while (!valid2);
                                                    valid2 = false;
                                                    do
                                                    {
                                                        Console.Write("Enter the % weight:");
                                                        string data = Console.ReadLine();
                                                        double temp;
                                                        valid2 = double.TryParse(data, out temp);
                                                        if (temp < 0 || temp>100)
                                                        {
                                                            valid2 = false;
                                                        }
                                                        if (valid2)
                                                            evaluation.Weight = temp;
                                                        else
                                                            Console.WriteLine("\tERROR: 'Weight' must be a postive number that is less than 100");
                                                    } while (!valid2);
                                                    valid2 = false;
                                                    do
                                                    {
                                                        Console.Write("Enter the marks earned or press ENTER to skip:");
                                                        string data = Console.ReadLine();
                                                        if (data == "")
                                                        {
                                                            evaluation.EarnedMarks = null;
                                                            valid2 = true;
                                                        }
                                                        else
                                                        {
                                                            double temp;
                                                            valid2 = double.TryParse(data, out temp);
                                                            if (temp < 0)
                                                            {
                                                                valid2 = false;
                                                            }
                                                            if (valid2)
                                                                evaluation.EarnedMarks = temp;
                                                            else
                                                                Console.WriteLine("\tERROR: 'Marks earned' must be a postive number or null");
                                                        }
                                                    } while (!valid2);
                                                    //Making sure evaluation list exists
                                                    if (Courses[tempInput - 1].Evalulations == null)
                                                    {
                                                        Courses[tempInput - 1].Evalulations = new List<Evaluation>();
                                                    }
                                                    //Adding evalution
                                                    Courses[tempInput - 1].Evalulations.Add(evaluation);
                                                    //Validating Courses
                                                    valid = ValidateItem(Courses[tempInput - 1], json_schema);

                                                    if (!valid)
                                                        Console.WriteLine("\nERROR:\tItem data does not match the required format. Please try again.\n");
                                                } while (!valid);//End of validation loop
                                            }
                                            //Cancel
                                            else if (input2.Equals("X"))
                                            {
                                                done2 = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Printing out evaluations
                                        Console.Write("#. Evaluation\t");
                                        Console.Write("Marks Earned\t");
                                        Console.Write("Out Of\t");
                                        Console.Write("Percent\t");
                                        Console.Write("Course Marks\t");
                                        Console.WriteLine("Weight/100\n");
                                        //Console.WriteLine("GPA\n");
                                        for (int i = 0; i < Courses[tempInput - 1].Evalulations.Count; i++)
                                        {
                                            Console.Write(i + 1 + ". ");
                                            Console.Write(Courses[tempInput - 1].Evalulations[i].Description + "\t");
                                            //Making sure marks earned is there
                                            if (Courses[tempInput - 1].Evalulations[i].EarnedMarks == null)
                                            {
                                                Console.Write("\t\t");
                                                Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[i].OutOf) + "\t");
                                                Console.Write("0.0\t\t");
                                                Console.Write("0.0\t\t");
                                            }
                                            else
                                            {
                                                Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[i].EarnedMarks) + "\t\t");
                                                Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[i].OutOf) + "\t");
                                                Console.Write(String.Format("{0:0.0}", CalcCoursePercent(Courses[tempInput - 1].Evalulations[i].EarnedMarks, Courses[tempInput - 1].Evalulations[i].OutOf)) + "\t\t");
                                                Console.Write(String.Format("{0:0.0}", CalcCourseMarks(CalcCoursePercent(Courses[tempInput - 1].Evalulations[i].EarnedMarks, Courses[tempInput - 1].Evalulations[i].OutOf), Courses[tempInput - 1].Evalulations[i].Weight)) + "\t\t");
                                            }
                                            Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[i].Weight) + "\n");
                                        }
                                        Console.Write("\nGPA: ");
                                        Console.Write(String.Format("{0:0.0}", CalcGPAforClass(Courses[tempInput - 1])));
                                        PrintInfo2();
                                        object input2 = Console.ReadLine();
                                        int tempInput3 = 0;
                                            //Checking if input is a number
                                            if (int.TryParse((string)input2, out tempInput3))
                                            {
                                                //making sure number is valid
                                                if (Courses[tempInput - 1].Evalulations.Count > 0)
                                                {
                                                    bool done3 = false;
                                                    bool valid3;
                                                    if (tempInput3 > 0 && tempInput3 <= Courses[tempInput - 1].Evalulations.Count)
                                                    {
                                                        do
                                                        {
                                                            //Printing out that evalutions numbers
                                                            Console.Clear();
                                                            PrintTitle();
                                                            Console.Write("Marks Earned\t");
                                                            Console.Write("Out Of\t");
                                                            Console.Write("Percent\t");
                                                            Console.Write("Course Marks\t");
                                                            Console.WriteLine("Weight/100\n");
                                                            //Checking if earned marks is null
                                                            if (Courses[tempInput - 1].Evalulations[tempInput3 - 1].EarnedMarks == null)
                                                            {
                                                                Console.Write("\t\t");
                                                                Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf) + "\t");
                                                                Console.Write("0.0\t\t");
                                                                Console.Write("0.0\t\t");
                                                            }
                                                            else
                                                            {
                                                                Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput3 - 1].EarnedMarks) + "\t\t");
                                                                Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf) + "\t");
                                                                Console.Write(String.Format("{0:0.00}", CalcCoursePercent(Courses[tempInput - 1].Evalulations[tempInput3 - 1].EarnedMarks, Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf)) + "\t\t");
                                                                Console.Write(String.Format("{0:0.0}", CalcCourseMarks(CalcCoursePercent(Courses[tempInput - 1].Evalulations[tempInput3 - 1].EarnedMarks, Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf), Courses[tempInput - 1].Evalulations[tempInput3 - 1].Weight)) + "\t\t");
                                                            }
                                                            Console.Write(String.Format("{0:0.0}", Courses[tempInput - 1].Evalulations[tempInput3 - 1].Weight) + "\n");
                                                            PrintInfo3();
                                                            object input3 = Console.ReadLine();
                                                            //Delete
                                                            if (input3.Equals("D"))
                                                            {
                                                                //Checking for confirmation
                                                                Console.Write("Delete " + Courses[tempInput - 1].Evalulations[tempInput3 - 1].Description + "? (Y/N):");
                                                                string data = Console.ReadLine();
                                                                if (data.Equals("Y"))
                                                                {
                                                                    //Deleting evaluation if list is big enough
                                                                    if (Courses[tempInput - 1].Evalulations.Count > 1)
                                                                    {
                                                                        Courses[tempInput - 1].Evalulations.RemoveAt(tempInput3 - 1);

                                                                    }
                                                                    //Deleting evaluation list if only evaluation
                                                                    else
                                                                    {
                                                                        Courses[tempInput - 1].Evalulations = null;
                                                                    }
                                                                    done3 = true;
                                                                }
                                                            }
                                                            //Edit
                                                            else if (input3.Equals("E"))
                                                            {
                                                                do
                                                                {
                                                                    //Asking for new marks earned
                                                                    Console.Write("Enter the marks earned out of " + Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf + ", press ENTER to leave unassigned:");
                                                                    string data = Console.ReadLine();
                                                                    //Validating data
                                                                    if (data == "")
                                                                    {
                                                                        evaluation.EarnedMarks = null;
                                                                        valid3 = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        double temp;
                                                                        valid3 = double.TryParse(data, out temp);
                                                                        if (temp < 0 || temp > Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf)
                                                                        {
                                                                            valid3 = false;
                                                                        }
                                                                        if (valid3)
                                                                            evaluation.EarnedMarks = temp;
                                                                        else
                                                                            Console.WriteLine("\tERROR: 'Marks earned' must be a postive number that is less than " + Courses[tempInput - 1].Evalulations[tempInput3 - 1].OutOf + " or null");
                                                                    }
                                                                } while (!valid3);//End of validation loop
                                                                                  //Setting new marks earned
                                                                Courses[tempInput - 1].Evalulations[tempInput3 - 1].EarnedMarks = evaluation.EarnedMarks;
                                                                //Validating courses
                                                                valid3 = ValidateItem(Courses[tempInput - 1], json_schema);

                                                                if (!valid3)
                                                                    Console.WriteLine("\nERROR:\tItem data does not match the required format. Please try again.\n");


                                                            }
                                                            //Cancel
                                                            else if (input3.Equals("X"))
                                                            {
                                                                done3 = true;
                                                            }

                                                        } while (!done3);//End of input loop
                                                    }
                                                    //Error for bad number
                                                    else
                                                    {
                                                        Console.WriteLine("\nERROR:\tThat course doesn't exist, please input a new number.\n");
                                                    }
                                                }
                                            }
                                            //Checking second input to see if string
                                            else if (input2.GetType() == typeof(string))
                                            {
                                                //Delete
                                                if (input2.Equals("D"))
                                                {
                                                    //Asking for confirmation
                                                    Console.Write("Delete " + Courses[tempInput - 1].Code + "? (Y/N):");
                                                    string data = Console.ReadLine();
                                                    //Removing course
                                                    if (data.Equals("Y"))
                                                    {
                                                        Courses.RemoveAt(tempInput - 1);
                                                        done2 = true;
                                                    }
                                                }
                                                //Add
                                                else if (input2.Equals("A"))
                                                {
                                                    do
                                                    {
                                                        //Asking for input and validating it
                                                        Console.Write("Enter a description:");
                                                        evaluation.Description = Console.ReadLine();
                                                        do
                                                        {
                                                            Console.Write("Enter the 'out of' mark:");
                                                            string data = Console.ReadLine();
                                                            int temp;
                                                            valid2 = int.TryParse(data, out temp);
                                                            if (temp < 0)
                                                            {
                                                                valid2 = false;
                                                            }
                                                            if (valid2)
                                                                evaluation.OutOf = temp;
                                                            else
                                                                Console.WriteLine("\tERROR: 'Out of' must be a postive number");
                                                        } while (!valid2);
                                                        valid2 = false;
                                                        do
                                                        {
                                                            Console.Write("Enter the % weight:");
                                                            string data = Console.ReadLine();
                                                            double temp;
                                                            valid2 = double.TryParse(data, out temp);
                                                            if (temp < 0 || temp > 100)
                                                            {
                                                                valid2 = false;
                                                            }
                                                            if (valid2)
                                                                evaluation.Weight = temp;
                                                            else
                                                                Console.WriteLine("\tERROR: 'Weight' must be a postive number that is less than 100");
                                                        } while (!valid2);
                                                        valid2 = false;
                                                        do
                                                        {
                                                            Console.Write("Enter the marks earned or press ENTER to skip:");
                                                            string data = Console.ReadLine();
                                                            if (data == "")
                                                            {
                                                                evaluation.EarnedMarks = null;
                                                                valid2 = true;
                                                            }
                                                            else
                                                            {
                                                                double temp;
                                                                valid2 = double.TryParse(data, out temp);
                                                                if (temp < 0)
                                                                {
                                                                    valid2 = false;
                                                                }
                                                                if (valid2)
                                                                    evaluation.EarnedMarks = temp;
                                                                else
                                                                    Console.WriteLine("\tERROR: 'Marks earned' must be a postive number or null");
                                                            }
                                                        } while (!valid2);
                                                        //Checking if evaluations is null and making new evaluation list
                                                        if (Courses[tempInput - 1].Evalulations == null)
                                                        {
                                                            Courses[tempInput - 1].Evalulations = new List<Evaluation>();
                                                        }
                                                        //Adding new evaluation
                                                        Courses[tempInput - 1].Evalulations.Add(evaluation);
                                                        //Validating courses
                                                        valid = ValidateItem(Courses[tempInput - 1], json_schema);

                                                        if (!valid)
                                                            Console.WriteLine("\nERROR:\tItem data does not match the required format. Please try again.\n");
                                                    } while (!valid);//End of validation loop
                                                }
                                                //Cancel
                                                else if (input2.Equals("X"))
                                                {
                                                    done2 = true;
                                                }
                                                else if (input2.ToString().ToUpper().Equals("O"))
                                                {
                                                    Console.Clear();
                                                    string inputDesire = "";
                                                    Evaluation newEval = new Evaluation();
                                                    List<Item> future = new List<Item>();
                                                    Console.Write("#. Evaluation\t");
                                                    Console.Write("Marks Earned\t");
                                                    Console.Write("Out Of\t");
                                                    Console.Write("Percent\t");
                                                    Console.Write("Course Marks\t");
                                                    Console.WriteLine("Weight/100\n");

                                                    for (int i = 0; i < CourseGoal[tempInput - 1].Evalulations.Count; i++)
                                                    {
                                                        Console.Write(i + 1 + ". ");
                                                        Console.Write(CourseGoal[tempInput - 1].Evalulations[i].Description + "\t");
                                                        if (CourseGoal[tempInput - 1].Evalulations[i].EarnedMarks == null)
                                                        {
                                                            Console.Write("\t\t");
                                                            Console.Write(String.Format("{0:0.0}", CourseGoal[tempInput - 1].Evalulations[i].OutOf) + "\t");
                                                            Console.Write("0.0\t\t");
                                                            Console.Write("0.0\t\t");
                                                        }
                                                        else
                                                        {
                                                            Console.Write(String.Format("{0:0.0}", CourseGoal[tempInput - 1].Evalulations[i].EarnedMarks) + "\t\t");
                                                            Console.Write(String.Format("{0:0.0}", CourseGoal[tempInput - 1].Evalulations[i].OutOf) + "\t");
                                                            Console.Write(String.Format("{0:0.0}", CalcCoursePercent(CourseGoal[tempInput - 1].Evalulations[i].EarnedMarks, CourseGoal[tempInput - 1].Evalulations[i].OutOf)) + "\t\t");
                                                            Console.Write(String.Format("{0:0.0}", CalcCourseMarks(CalcCoursePercent(CourseGoal[tempInput - 1].Evalulations[i].EarnedMarks, CourseGoal[tempInput - 1].Evalulations[i].OutOf), CourseGoal[tempInput - 1].Evalulations[i].Weight)) + "\t\t");


                                                        }
                                                        Console.Write(String.Format("{0:0.0}", CourseGoal[tempInput - 1].Evalulations[i].Weight) + "\n");
                                                    }

                                                    Console.Write("\nPlease enter the grade you are aiming for in a percent: ");
                                                    string tempDes = Console.ReadLine();
                                                    double tempDesNum = double.Parse(tempDes);
                                                    double completedWeight = 0;
                                                    //for assignments with no earned marks
                                                    List<Evaluation> newL = new List<Evaluation>();
                                                    //with earned marks
                                                    List<Evaluation> anotherNewL = new List<Evaluation>();


                                                    double userHas = 0.0;
                                                    double totalCompletedSoFar = 0.0;

                                                    for (int i = 0; i < CourseGoal[tempInput - 1].Evalulations.Count; i++)
                                                    {
                                                        //Course weight completed
                                                        completedWeight += CourseGoal[tempInput - 1].Evalulations[i].Weight;
                                                        if (CourseGoal[tempInput - 1].Evalulations[i].EarnedMarks == null)
                                                        {
                                                            newEval = CourseGoal[tempInput - 1].Evalulations[i];
                                                            newL.Add(newEval);
                                                        }
                                                        else
                                                        {
                                                            anotherNewL.Add(CourseGoal[tempInput - 1].Evalulations[i]);
                                                        }
                                                    }

                                                    double?[] tempEarned = new double?[anotherNewL.Count];
                                                    double[] tempPotential = new double[anotherNewL.Count];
                                                    //Gathering all evaluations to do calculations
                                                    for (int j = 0; j < anotherNewL.Count; j++)
                                                    {
                                                        tempEarned[j] = anotherNewL[j].EarnedMarks;
                                                        tempPotential[j] = anotherNewL[j].OutOf;
                                                        totalCompletedSoFar += anotherNewL[j].Weight;
                                                    }
                                                    userHas = CalcCoursePercent(CalcCourseTotalEarned(tempEarned), CalcCourseTotalPotential(tempPotential));
                                                
                                                    //course weight left
                                                    double weightLeft = 100 - totalCompletedSoFar;
                                                    double doubleby = weightLeft/100.0 ;
                                                    userHas = userHas * doubleby;
                                                    double numDiff = tempDesNum - userHas;
                                                    Console.WriteLine("");
                                                    if (newL.Count > 0)
                                                    {
                                                        for (int i = 0; i < newL.Count; i++)
                                                        {
                                                            double testagain = Math.Round(newL[i].OutOf * (tempDesNum / 100.0) * (1.0 + (numDiff / 100.0)), 2);
                                                            Console.WriteLine("For " + newL[i].Description + " you would need " + testagain + " out of " + newL[i].OutOf + " for an " + tempDesNum + "%.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        double testagain = Math.Round((numDiff/100.0)/(weightLeft/100.0), 2);
                                                        Console.WriteLine("For your final assignment you would need " + testagain*100 + " out of 100 for an " + tempDesNum + "%.");
                                                    }

                                                    Console.Write("Press enter to stop reading:");
                                                    Console.Read();
                                                }
                                            }
                                    }
                                } while (!done2);//End of second screen

                            }
                            //Making sure course number is correct
                            else
                            {
                                Console.WriteLine("\nERROR:\tThat course doesn't exist, please input a new number.\n");
                            }
                        }
                        //Making sure courses exist before number is selected
                        else
                        {
                            Console.WriteLine("\nERROR:\tNo courses exist, please input a new course.\n");
                        }
                    }
                    //Checking if input is string
                    else if (input.GetType() == typeof(string))
                    {
                        //Add
                        if (input.Equals("A"))
                        {
                            do
                            {
                                //Asking for code input and validating
                                Console.Write("\nEnter a course code: ");
                                item.Code = Console.ReadLine();
                                valid = ValidateItem(item, json_schema);

                                if (!valid)
                                    Console.WriteLine("\nERROR:\tItem data does not match the required format. Please try again.\n");

                            } while (!valid);//End of valid loop
                            //Adding item to courses
                            Courses.Add(item);
                        }
                        //Cancel
                        else if (input.Equals("X"))
                        {
                            done = true;
                        }
                    }
                } while (!done);//End of main screen
                //Validate all object of courses incase i missed a validation
                for (int i = 0; i < Courses.Count; i++)
                {
                    if (!ValidateItem(Courses[i], json_schema))
                    {
                        Console.WriteLine("Courses list does not comply with json_schema.\nPlease modify your courses file");
                        Environment.Exit(1);
                    }
                }
                //Writing list of courses to json file
                string returnFile = JsonConvert.SerializeObject(Courses);
                string Directory = Environment.CurrentDirectory + @"\returngrades.json";
                File.WriteAllText(Directory, returnFile);
            }
            //Making sure schema file exists
            else
            {
                Console.WriteLine("\nERROR:\tUnable to read the schema file.");
            }
        }
        /*
	        Name: PrintTitle
	        Input: void
	        Output: void
	        Description: Prints the title for each screen
        */
        static public void PrintTitle()
        {
            Console.WriteLine("\t\t~ SLOW DELIVERY'S GRADE CALCULATOR ~\n");
            Console.WriteLine("+----------------------------------------------------------------+");
            Console.WriteLine("|\t\t\tGrades Summary\t\t\t\t |");
            Console.WriteLine("+----------------------------------------------------------------+");
            Console.WriteLine();
        }
        /*
	        Name: PrintInfo
	        Input: void
	        Output: void
	        Description: Prints the command info for the first screen
        */
        static public void PrintInfo()
        {
            Console.WriteLine("\n------------------------------------------------------------------");
            Console.WriteLine("Press # from the above list to view/edit/delete a specific course.");
            Console.WriteLine("Press A to add a new course.");
            Console.WriteLine("Press X to quit.");
            Console.WriteLine("------------------------------------------------------------------");
            Console.Write("Enter a command: ");
        }
        /*
	        Name: PrintInfo2
	        Input: void
	        Output: void
	        Description: Prints the command info for the second screen
        */
        static public void PrintInfo2()
        {
            Console.WriteLine("\n------------------------------------------------------------------");
            Console.WriteLine("Press D to delete this course.");
            Console.WriteLine("Press A to add an evaluation.");
            Console.WriteLine("Press # from the above list to edit/delete a specific evaluation.");
                Console.WriteLine("Press O to determine what it takes for your desired grade.");
                Console.WriteLine("Press X to quit.");
            Console.WriteLine("------------------------------------------------------------------");
            Console.Write("Enter a command: ");
        }
        /*
	        Name: PrintInfo2
	        Input: void
	        Output: void
	        Description: Prints the command info for the third screen
        */
        static public void PrintInfo3()
        {
            Console.WriteLine("\n------------------------------------------------------------------");
            Console.WriteLine("Press D to delete this evaluation.");
            Console.WriteLine("Press E to edit this evaluation.");
            Console.WriteLine("Press X to quit.");
            Console.WriteLine("------------------------------------------------------------------");
            Console.Write("Enter a command: ");
        }
            static public void PrintInfo4()
            {
                Console.WriteLine("\n------------------------------------------------------------------");
                Console.WriteLine("Press E to enter another marked task.");
                Console.WriteLine("Press D to delete a marked task.");
                Console.WriteLine("Press A to determine marks needed");
                Console.WriteLine("------------------------------------------------------------------");
                Console.Write("Enter a command: ");
            }
            /*
                Name: CalcCourseTotalEarned
                Input: double?[]
                Output: doube
                Description: Calculates the total earned marks for a course
            */
            static public double CalcCourseTotalEarned(double?[] earnedMarks)
        {
            double total = 0;
            for (int i = 0; i < earnedMarks.Length; i++)
            {
                if (earnedMarks != null)
                {
                    total = total + (double)earnedMarks[i];
                }
            }
            return total;
        }
        /*
	        Name: CalcCourseTotalPotential
	        Input: double[]
	        Output: doube
	        Description: Calculates the total potential marks for a course
        */
        static public double CalcCourseTotalPotential(double[] potentialMarks)
        {
            double total = 0;
            for (int i = 0; i < potentialMarks.Length; i++)
            {
                total = total + potentialMarks[i];
            }
            return total;
        }
        /*
	        Name: CalcCourseMarks
	        Input: double, double
	        Output: doube
	        Description: Calculates how many marks a person earned for the course from a specific evaluation
        */
        static public double CalcCourseMarks(double percentage, double weight)
        {
            double total = 0;
            total = weight * percentage / 100;
            return total;
        }
        /*
	        Name: CalcCoursePercent
	        Input: double?
	        Output: doube
	        Description: Calculates the percent for a given assignment
        */
        static public double CalcCoursePercent(double? totalEarnedMarks, double totalPotentialMarks)
        {
            double total = 0;
            if (totalEarnedMarks != null)
            {
                total = ((double)totalEarnedMarks / totalPotentialMarks) * 100;
            }
            return total;
        }
        /*
           Name: CalcGPAforClass
           Input: course item
           Output: double
           Description: Goes through all assignments in a course and adds the Student's Marks to get the total and the GPA equivalent is returned.
       */
        public static double CalcGPAforClass(Item course)
        {
            double gradePoint = 0;
            double assiGrade = 0;
 


            foreach (Evaluation assignment in course.Evalulations)
            {
                assiGrade = CalcCoursePercent(assignment.EarnedMarks, assignment.OutOf);
            }
            if (assiGrade >= 90.0)
            {
                gradePoint = 4.2;
            }

            if (assiGrade >= 80.0 && assiGrade < 90.0)
            {
                gradePoint = 4.0;
            }

            if (assiGrade >= 75.0 && assiGrade < 80.0)
            {
                gradePoint = 3.5;
            }

            if (assiGrade >= 70.0 && assiGrade < 75.0)
            {
                gradePoint = 3.0;
            }

            if (assiGrade >= 65.0 && assiGrade < 70.0)
            {
                gradePoint = 2.5;
            }

            if (assiGrade >= 60.0 && assiGrade < 65.0)
            {
                gradePoint = 2.0;
            }

            if (assiGrade >= 55.0 && assiGrade < 60.0)
            {
                gradePoint = 1.5;
            }

            if (assiGrade >= 50.0 && assiGrade < 55.0)
            {
                gradePoint = 1.0;
            }


            if (assiGrade < 50.0)
            {
                gradePoint = 0.0;
            }

            return gradePoint;
        }
        /*
	        Name: ValidateItem
	        Input: item, String
	        Output: bool
	        Description: Validates an item against a json schema
        */
        private static bool ValidateItem(Item item, string json_schema)
        {
            // Convert item object to a JSON string 
            string json_data = JsonConvert.SerializeObject(item);

            // Validate the data string against the schema contained in the 
            // json_schema parameter. Also, modify or replace the following 
            // return statement to return 'true' if item is valid, or 'false' 
            // if invalid.
            JSchema schema = JSchema.Parse(json_schema);
            JObject itemObj = JObject.Parse(json_data);
            return itemObj.IsValid(schema);

        } // end ValidateItem()
        /*
	        Name: ReadFile
	        Input: string, out string
	        Output: bool
	        Description: Reads a file given and gives it back in string format
        */
        private static bool ReadFile(string path, out string json)
        {
            try
            {
                // Read JSON file data 
                json = File.ReadAllText(path);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        } // end ReadFile
        /*
            Name:        askUser
            Input:       void
            Output:      void
            Description: Prompts user to create new .json files or use existing ones.
        */
        static public void AskUser()
        {
            bool temp = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Do you want to create new files?(Y/N)\n");
                Console.WriteLine("(Y) - Yes, create new files.");
                Console.WriteLine("(N) - No, use existing files.");
                Console.WriteLine();
                Console.Write("Enter a command: ");
                String userInput = Console.ReadLine();
                if (userInput == "Y")
                {
                    Console.WriteLine("\nNew files were created.");
                    temp = true;
                }
                else if (userInput == "N")
                {
                    Console.Clear();
                    bool temp2 = false;
                    do
                    {
                        Console.WriteLine("Files available");
                        Console.WriteLine("1.\tgrades.json");
                        Console.WriteLine("\n------------------------------------------------------------------");
                        Console.WriteLine("Press # to use that file.");
                        Console.WriteLine("Press X to go back");
                        Console.WriteLine("------------------------------------------------------------------");
                        Console.Write("Enter a command: ");
                        string userInput2 = Console.ReadLine();
                        if (userInput2 == "1")
                        {
                            temp2 = true;
                            temp = true;
                        }
                        else if (userInput2 == "X")
                        {
                            temp2 = true;
                        }
                        else {
                            Console.WriteLine("\nError...Please enter '#' or 'X'.");
                        }
                    } while (!temp2);
                }
                else
                {
                    Console.WriteLine("\nError...Please enter 'Y' or 'N'.");
                }
            } while (temp == false);
        }
        public static double CalcGPAforAllClasses(double[] gpa)
        {
            double total = 0;
            for (int i = 0; i < gpa.Length; i++)
            {
                total = total + gpa[i];
            }
            total /= gpa.Length;
            return total;
        }

    }//End of class GradeTracker

    //Defination for Item
    class Item
    {
        public string Code { get; set; }
        public List<Evaluation>? Evalulations { get; set; }
    } // end class Item

    //Defination for CourseHolder
    class CourseHolder
    {
        public List<Item> courses { get; set; }

    } // end class CourseHolder

    //Defination for Evalution
    class Evaluation
    {
        public string Description { get; set; }
        public double Weight { get; set; }
        public int OutOf { get; set; }
        public double? EarnedMarks { get; set; }

    } // end calss Evalution 
}
