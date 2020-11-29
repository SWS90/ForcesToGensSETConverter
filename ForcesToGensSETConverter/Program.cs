using HedgeLib.Sets;
using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Collections.Generic;

namespace ForcesToGensSETConverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-us");
            var forcesTemplates = SetObjectType.LoadObjectTemplates("Templates", "Forces");
            var gensTemplates = SetObjectType.LoadObjectTemplates("Templates", "Generations");
            var unknownObjects = new GensSetData();

            if (args.Length < 1 || !File.Exists(args[0]))
            {
                ShowHelp();
                return;
            }

            string text = args[0];
            string extension = Path.GetExtension(text);
            if (extension == null || extension.ToLower() != ForcesSetData.Extension)
            {
                ShowHelp();
                return;
            }

            // Load Forces Templates
            Console.WriteLine("Loading Forces Templates...");
            //string templateDir = Path.GetFullPath("Templates");
            if (!Directory.Exists(Path.Combine("Templates\\Forces")))
            {
                var oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NO TEMPLATES WERE FOUND FOR FORCES, ABORTING!!");
                Console.ForegroundColor = oldCol;
                Console.ReadKey();
                return;
            }

            if (gensTemplates.Count == 0)
            {
                var oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NO TEMPLATES WERE FOUND FOR GENERATIONS, ABORTING!!");
                Console.ForegroundColor = oldCol;
                Console.ReadKey();
                return;
            }

            // Convert Sets
            GensSetData gensSetData = new GensSetData();
            ForcesSetData forcesSetData = new ForcesSetData();
            forcesSetData.Load(text, forcesTemplates);

            foreach (SetObject forcesObj in forcesSetData.Objects)
            {
                SetObject gensObj = null;
                if (forcesObj.ObjectType == "ObjRing")
                {
                    gensObj = new SetObject(gensTemplates["Ring"], "Ring", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[6] = forcesObj.Parameters[0]; //ResetTime

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                    forcesObj.Transform.Position.Y += 0.3f;
                }
                else if (forcesObj.ObjectType == "ObjDashRing")
                {
                    gensObj = new SetObject(gensTemplates["DashRing"], "DashRing", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[3].Data) / 10; // FirstSpeed
                    gensObj.Parameters[7].Data = ((float)forcesObj.Parameters[2].Data) * 10; // KeepVelocityDistance
                    gensObj.Parameters[8].Data = ((float)forcesObj.Parameters[1].Data); // OutOfControl

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;

                    if (forcesObj.Transform.Rotation.X != 0)
                    {
                        float Pitch_Add5_DashRing = (float)(5 / 180.0 * Math.PI);
                        gensObj.Transform.Rotation = Quaternion.Multiply(gensObj.Transform.Rotation, Quaternion.CreateFromYawPitchRoll(0, Pitch_Add5_DashRing, 0)); //Y,X,Z - (yaw, pitch, roll)
                    }
                }

                else if (forcesObj.ObjectType == "ObjSpring")
                {
                    gensObj = new SetObject(gensTemplates["Spring"], "Spring", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.UseGensVector3 = true;
                    gensObj.Parameters[3].Data = ((float)forcesObj.Parameters[0].Data) / 10; // FirstSpeed
                    gensObj.Parameters[16].Data = ((float)forcesObj.Parameters[2].Data) / 10; // KeepVelocityDistance
                    gensObj.Parameters[18].Data = ((float)forcesObj.Parameters[1].Data); // OutOfControl
                    gensObj.Parameters[25].Data = gensObj.GensVector3; // m_MonkeyTarget

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }

                else if (forcesObj.ObjectType == "ObjWideSpring")
                {
                    gensObj = new SetObject(gensTemplates["WideSpring"], "WideSpring", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[0].Data) / 10; // FirstSpeed
                    gensObj.Parameters[6].Data = ((float)forcesObj.Parameters[2].Data) / 10; // KeepVelocityDistance
                    gensObj.Parameters[7].Data = ((float)forcesObj.Parameters[1].Data); // OutOfControl

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }

                else if (forcesObj.ObjectType == "ObjDashPanel")
                {
                    gensObj = new SetObject(gensTemplates["DashPanel"], "DashPanel", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[5].Data = !((bool)forcesObj.Parameters[2].Data); // IsInvisible
                    gensObj.Parameters[8].Data = ((float)forcesObj.Parameters[0].Data); // OutOfControl
                    gensObj.Parameters[10].Data = ((float)forcesObj.Parameters[1].Data) / 10; // Speed

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjBooster")
                {
                    gensObj = new SetObject(gensTemplates["DashPanel"], "DashPanel", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[5].Data = !((bool)forcesObj.Parameters[2].Data); // IsInvisible
                    gensObj.Parameters[8].Data = ((float)forcesObj.Parameters[0].Data); // OutOfControl
                    gensObj.Parameters[10].Data = ((float)forcesObj.Parameters[1].Data) / 10; // Speed

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }

                else if (forcesObj.ObjectType == "ObjJumpBoard")
                {
                    if ((byte)forcesObj.Parameters[5].Data == 0)
                    {
                        gensObj = new SetObject(gensTemplates["JumpBoard"], "JumpBoard", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                        gensObj.ObjectType = "JumpBoard";
                        gensObj.Parameters[0].Data = 1f; // AngleType
                        gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[1].Data) / 10; // ImpulseSpeedOnBoost
                        gensObj.Parameters[3].Data = ((float)forcesObj.Parameters[0].Data) / 10; // ImpulseSpeedOnNorml
                        gensObj.Parameters[7].Data = ((float)forcesObj.Parameters[2].Data); // OutOfControl}
                        gensObj.Parameters[9].Data = true; // RigidBody
                    }
                    if ((byte)forcesObj.Parameters[5].Data == 1)
                    {
                        gensObj = new SetObject(gensTemplates["JumpBoard3D"], "JumpBoard3D", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                        gensObj.ObjectType = "JumpBoard3D";
                        gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[1].Data) / 10; // ImpulseSpeedOnBoost
                        gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[0].Data) / 10; // ImpulseSpeedOnNorml
                        gensObj.Parameters[6].Data = ((float)forcesObj.Parameters[2].Data); // OutOfControl
                        gensObj.Parameters[8].Data = true; // RigidBody
                        gensObj.Parameters[9].Data = 0f; // SizeType
                    }
                    if ((byte)forcesObj.Parameters[5].Data == 2)
                    {
                        gensObj = new SetObject(gensTemplates["JumpBoard3D"], "JumpBoard3D", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                        gensObj.ObjectType = "JumpBoard3D";
                        gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[1].Data) / 10; // ImpulseSpeedOnBoost
                        gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[0].Data) / 10; // ImpulseSpeedOnNorml
                        gensObj.Parameters[6].Data = ((float)forcesObj.Parameters[2].Data); // OutOfControl
                        gensObj.Parameters[8].Data = true; // RigidBody
                        gensObj.Parameters[9].Data = 1f; // SizeType
                    }
                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                    float Yaw_trans_JumpBoard = (float)(180 / 180.0 * Math.PI);
                    gensObj.Transform.Rotation = Quaternion.Multiply(gensObj.Transform.Rotation, Quaternion.CreateFromYawPitchRoll(Yaw_trans_JumpBoard, 0, 0)); //Y,X,Z
                }

                else if (forcesObj.ObjectType == "ObjCameraVolume")
                {
                    gensObj = new SetObject(gensTemplates["ChangeVolumeCamera"], "ChangeVolumeCamera", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[12].Data) / 10; // Collision_Height - Forces Heigh
                    gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[13].Data) / 10; // Collision_Length - Forces Depth
                    gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[11].Data) / 10; // Collision_Width - Forces Width
                    gensObj.Parameters[3].Data = ((byte)forcesObj.Parameters[7].Data); // DefaultStatus
                    gensObj.Parameters[4].Data = ((float)forcesObj.Parameters[3].Data); // Ease_Time_Enter
                    gensObj.Parameters[5].Data = ((float)forcesObj.Parameters[4].Data); // Ease_Time_Leave
                    gensObj.Parameters[11].Data = ((uint)forcesObj.Parameters[1].Data); // Priority
                    gensObj.Parameters[14].Data = ((ForcesSetData.ObjectReference)forcesObj.Parameters[0].Data); // Target

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }

                else if (forcesObj.ObjectType == "ObjCameraSubVolume")
                {
                    gensObj = new SetObject(gensTemplates["ChangeVolumeCamera"], "ChangeVolumeCamera", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[4].Data) / 10; // Collision_Height - Forces Height
                    gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[5].Data) / 10; // Collision_Length - Forces Depth
                    gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[3].Data) / 10; // Collision_Width - Forces Width
                    gensObj.Parameters[14].Data = ((ForcesSetData.ObjectReference)forcesObj.Parameters[0].Data); // Target

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }

                else if (forcesObj.ObjectType == "EnemyEggPawn")
                {
                    gensObj = new SetObject(gensTemplates["EnemyEFighter3D"], "EnemyEFighter3D", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.UseGensVector3 = true;
                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                // Old entries by Rad i've yet to convert...
                /*else if (forcesObj.ObjectType == "ObjClassicSpring")
                {
                    gensObj = new SetObject(gensTemplates["SpringClassic"],
                        "SpringClassic", forcesObj.ObjectID);

                    gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[0].Data) / 10;
                    gensObj.Parameters[15] = forcesObj.Parameters[1];
                }*/
                if (gensObj == null) // Objects not in code
                {
                    var unknownObj = new SetObject();
                    unknownObj.ObjectType = forcesObj.ObjectType;
                    unknownObj.Transform = forcesObj.Transform;
                    unknownObj.Transform.Position /= 10;
                    unknownObjects.Objects.Add(unknownObj);
                }
                if (gensObj != null) // Objects in code
                {
                    gensSetData.Objects.Add(gensObj);
                }
            }
            // Save Gens Set Data
            Console.WriteLine("Done Converting Objects!");
            string directoryName = Path.GetDirectoryName(text);
            string fileName = Path.GetFileName(text);
            string str = fileName.Substring(0, fileName.IndexOf('.'));
            string filePath = Path.Combine(directoryName, "setdata_" + str + ".set.xml");
            string filePath2 = Path.Combine(directoryName, "setdata_unconverted_" + str + ".set.xml");
            Console.WriteLine("Saving Gens .set.xml...");
            gensSetData.Save(filePath, gensTemplates);
            Console.WriteLine(filePath + " " + "saved!");
            Console.WriteLine("Saving unconverted objects to a new .set.xml...");
            unknownObjects.Save(filePath2, gensTemplates);
            Console.WriteLine(filePath2 + " " + "saved!");
            Console.WriteLine("Done!\nPress any key to close...");
            Console.ReadKey();
        }
        public static void ShowHelp()
        {
            Console.WriteLine("ForcesToGensSETConverter");
            Console.WriteLine("This is a WIP, not all Forces objects will convert to Gens\nUnsupported objects will convert to a new .set.xml with the forces object names, position, and rotation.");
            Console.WriteLine("Originally made by Radfordhound to convert some forces objects, extended by SWS90.\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("Drag and drop a Forces .gedit onto ForcesToGensSETConverter.exe to get a .set.xml file for use in Generations.");
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}