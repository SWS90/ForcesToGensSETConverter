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
            var objectPhysicsObjects = new GensSetData();

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
                SetObject gensObjPhysics = null;
                //Common objects start
                if (forcesObj.ObjectType == "ObjRing")
                {
                    gensObj = new SetObject(gensTemplates["Ring"], "Ring", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[6].Data = forcesObj.Parameters[0].Data; //ResetTime
                    
                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                    forcesObj.Transform.Position.Y += 0.3f;
                }
                else if (forcesObj.ObjectType == "ObjSuperRing")
                {
                    gensObj = new SetObject(gensTemplates["SuperRing"], "SuperRing", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    
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
                else if (forcesObj.ObjectType == "ObjLinkedSpring")
                {
                    gensObj = new SetObject(gensTemplates["Spring"], "Spring", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.UseGensVector3 = true;
                    var Normal = (SetObjectParamGroup)(forcesObj.Parameters[7]); // <Normal> ParamGroup in ObjLinkedSpring.xml
                    var Path = (SetObjectParamGroup)(forcesObj.Parameters[8]); // <Path> ParamGroup in ObjLinkedSpring.xml
                    if ((byte)forcesObj.Parameters[3].Data == 0) //Normal Behavior
                    { 
                        gensObj.Parameters[3].Data = ((float)Normal.Parameters[0].Data) / 10; // FirstSpeed
                        gensObj.Parameters[16].Data = ((float)Normal.Parameters[2].Data) / 10; // KeepVelocityDistance
                        gensObj.Parameters[18].Data = ((float)Normal.Parameters[1].Data); // OutOfControl
                    }
                    if ((byte)forcesObj.Parameters[3].Data == 1) //Path Behavior
                    {
                        gensObj.Parameters[3].Data = ((float)Path.Parameters[0].Data) / 10; // FirstSpeed
                    }
                    
                    // Rangeom
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjSkySpring")
                {
                    gensObj = new SetObject(gensTemplates["AirSpring"], "AirSpring", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.UseGensVector3 = true;
                    gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[0].Data) / 10; // FirstSpeed
                    gensObj.Parameters[13].Data = ((float)forcesObj.Parameters[2].Data) / 10; // KeepVelocityDistance
                    gensObj.Parameters[15].Data = ((float)forcesObj.Parameters[1].Data); // OutOfControl

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
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) /10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjGrindBooster")
                {
                    gensObj = new SetObject(gensTemplates["GrindDashPanel"], "GrindDashPanel", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[4].Data = ((float)forcesObj.Parameters[0].Data) * 10; // OutOfControl
                    gensObj.Parameters[6].Data = ((float)forcesObj.Parameters[1].Data) / 10; // Speed

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
                        gensObj.Parameters[7].Data = ((float)forcesObj.Parameters[2].Data); // OutOfControl
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
                else if (forcesObj.ObjectType == "ObjPointMarker")
                {
                    gensObj = new SetObject(gensTemplates["PointMarker"], "PointMarker", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[2].Data = ((float)forcesObj.Parameters[1].Data) / 10; // Height
                    gensObj.Parameters[6].Data = ((float)forcesObj.Parameters[0].Data) / 10; // Width

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;

                }
                else if (forcesObj.ObjectType == "ObjStartPosition")
                {
                    gensObj = new SetObject(gensTemplates["SonicSpawn"], "SonicSpawn", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[0].Data = true; // Active
                    gensObj.Parameters[1].Data = "None"; // Mode

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjGismo")
                {
                    gensObjPhysics = new SetObject(gensTemplates["ObjectPhysics"], "ObjectPhysics", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObjPhysics.UseGensVector3 = true;
                    gensObjPhysics.Parameters[8].Data = ((string)forcesObj.Parameters[0].Data); // Type

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObjPhysics.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObjPhysics.Transform = forcesObj.Transform;
                    gensObjPhysics.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjGoalTrigger")
                {
                    gensObj = new SetObject(gensTemplates["GoalRing"], "GoalRing", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.UseGensVector3 = true;

                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    // Add Forces' ObjGoalTrigger PlateOffsetPos to position
                    gensObj.Transform.Position.X += ((Vector3)forcesObj.Parameters[13].Data).X;
                    gensObj.Transform.Position.Y += ((Vector3)forcesObj.Parameters[13].Data).Y;
                    gensObj.Transform.Position.Z += ((Vector3)forcesObj.Parameters[13].Data).Z;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjIronBox")
                {
                    gensObj = new SetObject(gensTemplates["ObjectPhysics"], "ObjectPhysics", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.UseGensVector3 = true;
                    gensObj.Parameters[4].Data = ((bool)forcesObj.Parameters[3].Data); // IsCastShadow
                    gensObj.Parameters[8].Data = ((string)"IronBox"); // Type
                    
                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjWispCapsule")
                {
                    if ((byte)forcesObj.Parameters[0].Data == 0) // Type = Wispon Power
                    {
                        //Do nothing, don't want to convert Wispon Power Type
                    }
                    if ((byte)forcesObj.Parameters[0].Data == 1) // Type = Boost Wisp
                    {
                        gensObj = new SetObject(gensTemplates["SuperRing"], "SuperRing", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);

                        // Range
                        float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                        gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                        // Transform
                        gensObj.Transform = forcesObj.Transform;
                        gensObj.Transform.Position /= 10;
                        forcesObj.Transform.Position.Y += 0.55f;
                    }
                }
                else if (forcesObj.ObjectType == "ObjBalloon2")
                {
                    gensObj = new SetObject(gensTemplates["Balloon"], "Balloon", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[1].Data = ((byte)forcesObj.Parameters[0].Data); // Dimension
                    gensObj.Parameters[4].Data = ((bool)forcesObj.Parameters[5].Data); // IsDefaultPositionRecover
                    gensObj.Parameters[7].Data = ((float)forcesObj.Parameters[4].Data); // ReviveTime
                    gensObj.Parameters[8].Data = ((float)forcesObj.Parameters[3].Data) / 10; // SpeedMax
                    gensObj.Parameters[9].Data = ((float)forcesObj.Parameters[2].Data) / 10; // SpeedMin
                    gensObj.Parameters[10].Data = ((float)forcesObj.Parameters[1].Data) / 10; // UpSpeed
                    
                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                //Common objects end

                //Camera objects start
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
                //Camera objects end

                //Enemy objects start
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
                //Enemy objects end

                //Trigger objects start
                else if (forcesObj.ObjectType == "AutorunTrigger")
                {
                    if ((byte)forcesObj.Parameters[0].Data == 0) // action = ACT_START
                    {
                        gensObj = new SetObject(gensTemplates["AutorunStartCollision"], "AutorunStartCollision", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                        gensObj.ObjectType = "AutorunStartCollision";
                        gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[3].Data) / 10; // Collision_Height - Forces Height
                        gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[2].Data) / 10; // Collision_Width - Forces Width
                        gensObj.Parameters[8].Data = ((float)forcesObj.Parameters[5].Data) / 10; // Speed
                    }
                    if ((byte)forcesObj.Parameters[0].Data == 1) // action = ACT_FINISH
                    {
                        gensObj = new SetObject(gensTemplates["AutorunFinishCollision"], "AutorunFinishCollision", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                        gensObj.ObjectType = "AutorunFinishCollision";
                        gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[3].Data) / 10; // Collision_Height - Forces Height
                        gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[2].Data) / 10; // Collision_Width - Forces Width
                    }
                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                //Trigger objects end

                // Sound objects start
                else if (forcesObj.ObjectType == "ObjPointSoundSource")
                {
                    gensObj = new SetObject(gensTemplates["SoundPoint"], "SoundPoint", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.WriteGensRangeAsNormalParam = true;
                    gensObj.WriteGensRangeAsSpecialParam = false;
                    gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[4].Data); // Volume
                    gensObj.Parameters[1].Data = ((string)forcesObj.Parameters[0].Data); // Cuename
                    gensObj.Parameters[9].Data = ((float)forcesObj.Parameters[5].Data) / 5; // Range
                    gensObj.Parameters[8].Data = ((float)gensObj.Parameters[9].Data) - 4; // Radius
                    gensObj.Parameters[4].Data = ((float)gensObj.Parameters[8].Data) - 13; // InsideRadius
                    
                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                else if (forcesObj.ObjectType == "ObjChangeBGMTrigger")
                {
                    gensObj = new SetObject(gensTemplates["ChangeBGMCollision"], "ChangeBGMCollision", forcesObj.ObjectID, forcesObj.TargetID, forcesObj.TargetPosition);
                    gensObj.Parameters[0].Data = ((float)forcesObj.Parameters[5].Data) / 10; // Collision_Height
                    gensObj.Parameters[1].Data = ((float)forcesObj.Parameters[4].Data) / 10; // Collision_Width
                    gensObj.Parameters[8].Data = ((float)forcesObj.Parameters[3].Data); // LerpTimeBack
                    gensObj.Parameters[9].Data = ((float)forcesObj.Parameters[3].Data); // LerpTimeFront


                    // Range
                    float range = forcesObj.GetCustomDataValue("RangeIn", 1000f) / 10;
                    gensObj.CustomData.Add("Range", new SetObjectParam(typeof(float), range));

                    // Transform
                    gensObj.Transform = forcesObj.Transform;
                    gensObj.Transform.Position /= 10;
                }
                // Sound objects end

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
                    if (forcesObj.ObjectType != "ObjGismo")
                    {
                        var unknownObj = new SetObject();
                        unknownObj.ObjectType = forcesObj.ObjectType;
                        unknownObj.ObjectID = forcesObj.ObjectID;
                        unknownObj.Transform = forcesObj.Transform;
                        unknownObj.Transform.Position /= 10;
                        unknownObjects.Objects.Add(unknownObj);
                    }
                }
                if (gensObj != null) // Objects in code
                {
                    gensSetData.Objects.Add(gensObj);
                    
                }
                if (gensObjPhysics != null) 
                {
                    if (forcesObj.ObjectType == "ObjGismo")
                    {
                        objectPhysicsObjects.Objects.Add(gensObjPhysics);
                    }
                }
            }
            // Save Gens Set Data
            Console.WriteLine("Done Converting Objects!");
            string directoryName = Path.GetDirectoryName(text);
            string fileName = Path.GetFileName(text);
            string forcesGeditFileName = fileName.Substring(0, fileName.IndexOf('.'));
            string supportedObjects = Path.Combine(directoryName, "setdata_" + forcesGeditFileName + ".set.xml");
            string unsupportedObjects = Path.Combine(directoryName, "setdata_Unconverted_" + forcesGeditFileName + ".set.xml");
            string supportedObjects_ObjPhysics = Path.Combine(directoryName, "setdata_ObjectPhysics_" + forcesGeditFileName + ".set.xml");
            if (gensSetData.Objects.Count > 0)
            {
                Console.WriteLine("Saving supported objects to .set.xml...");
                gensSetData.Save(supportedObjects, gensTemplates);
                Console.WriteLine(supportedObjects + " " + "saved!");
            }
            if (unknownObjects.Objects.Count > 0)
            {
                Console.WriteLine("Saving unconverted objects to .set.xml...");
                unknownObjects.Save(unsupportedObjects, gensTemplates);
                Console.WriteLine(unsupportedObjects + " " + "saved!");
            }
            if (objectPhysicsObjects.Objects.Count > 0)
            {
                Console.WriteLine("Saving ObjectPhysics objects to .set.xml...");
                objectPhysicsObjects.Save(supportedObjects_ObjPhysics, gensTemplates);
                Console.WriteLine(supportedObjects_ObjPhysics + " " + "saved!");
            }
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