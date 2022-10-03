#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.LowLevel;
using static UnityEngine.LowLevel.PlayerLoopSystem;

public static class PlayerLoopExtensions
{
    public static Type? GetParentSystemType<T>(this ref PlayerLoopSystem playerLoop)
    {
        Stack<PlayerLoopSystem> systems = new Stack<PlayerLoopSystem>();
        systems.Push(playerLoop);

        while (systems.Count > 0) //safe
        {
            PlayerLoopSystem parent = systems.Pop();
            if (parent.subSystemList is not null)
            {
                int subSystemCount = parent.subSystemList.Length;
                for (int i = 0; i < subSystemCount; i++)
                {
                    PlayerLoopSystem subSystem = parent.subSystemList[i];
                    if (subSystem.type == typeof(T))
                    {
                        return parent.type;
                    }

                    systems.Push(subSystem);
                }
            }
        }

        return null;
    }

    public static ref PlayerLoopSystem GetSystem(this ref PlayerLoopSystem playerLoop, Type type)
    {
        int length = playerLoop.subSystemList.Length;
        for (int i = 0; i < length; i++)
        {
            ref PlayerLoopSystem subSystemA = ref playerLoop.subSystemList[i];
            if (subSystemA.type == type)
            {
                return ref subSystemA;
            }

            int? lengthA = subSystemA.subSystemList?.Length;
            for (int a = 0; a < lengthA; a++)
            {
                ref PlayerLoopSystem subSystemB = ref subSystemA.subSystemList![a];
                if (subSystemB.type == type)
                {
                    return ref subSystemB;
                }

                int? lengthB = subSystemB.subSystemList?.Length;
                for (int b = 0; b < lengthB; b++)
                {
                    ref PlayerLoopSystem subSystemC = ref subSystemB.subSystemList![b];
                    if (subSystemC.type == type)
                    {
                        return ref subSystemC;
                    }

                    int? lengthC = subSystemC.subSystemList?.Length;
                    for (int c = 0; c < lengthC; c++)
                    {
                        ref PlayerLoopSystem subSystemD = ref subSystemC.subSystemList![c];
                        if (subSystemD.type == type)
                        {
                            return ref subSystemD;
                        }
                    }
                }
            }
        }

        throw new Exception("Player loop system type doesn't exist");
    }

    public static ref PlayerLoopSystem GetParentSystem<T>(this ref PlayerLoopSystem playerLoop)
    {
        Type? parentType = GetParentSystemType<T>(ref playerLoop);
        if (parentType is not null)
        {
            return ref GetSystem(ref playerLoop, parentType);
        }
        else
        {
            throw new Exception("Player loop system type doesn't exist");
        }
    }

    public static bool InjectAfter<T>(this ref PlayerLoopSystem playerLoop, UpdateFunction method, Type systemType)
    {
        ref PlayerLoopSystem parentSystem = ref GetParentSystem<T>(ref playerLoop);
        int length = parentSystem.subSystemList.Length;

        //disallow duplicates
        for (int i = 0; i < length; i++)
        {
            ref PlayerLoopSystem subSystem = ref parentSystem.subSystemList[i];
            if (subSystem.type == systemType)
            {
                return false;
            }
        }

        //find system T and inject after by inserting at i
        for (int i = 0; i < length; i++)
        {
            ref PlayerLoopSystem subSystem = ref parentSystem.subSystemList[i];
            if (subSystem.type == typeof(T))
            {
                PlayerLoopSystem customSystem = new PlayerLoopSystem();
                customSystem.type = systemType;
                customSystem.updateDelegate = method;

                List<PlayerLoopSystem> subSystemList = new List<PlayerLoopSystem>(parentSystem.subSystemList);
                subSystemList.Insert(i, customSystem);
                parentSystem.subSystemList = subSystemList.ToArray();
                return true;
            }
        }

        return false;
    }

    public static string GetPlayerLoopNames(this ref PlayerLoopSystem playerLoop, string indent = "    ")
    {
        StringBuilder builder = new StringBuilder();
        if (playerLoop.subSystemList is not null)
        {
            //print the entire tree using a stack for recursion
            Stack<(int, PlayerLoopSystem)> stack = new Stack<(int, PlayerLoopSystem)>();
            stack.Push((0, playerLoop));
            while (stack.Count > 0) //safe
            {
                (int depth, PlayerLoopSystem system) = stack.Pop();

                //print the name of the system
                if (system.type is not null)
                {
                    for (int i = 0; i < depth; i++)
                    {
                        builder.Append(indent);
                    }

                    builder.Append(system.type.FullName);
                    builder.AppendLine();
                }
                else
                {
                    depth--;
                }

                //add the subsystems to the stack
                if (system.subSystemList is not null)
                {
                    int length = system.subSystemList.Length;
                    for (int s = 0; s < length; s++)
                    {
                        PlayerLoopSystem subSystem = system.subSystemList[s];
                        stack.Push((depth + 1, subSystem));
                    }
                }
            }
        }

        return builder.ToString();
    }
}