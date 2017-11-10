﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DollsLang
{
    public partial class Runtime
    {
        private StringBuilder outputBuffer;
        private Random random;

        private void InitializeRuntime()
        {
            outputBuffer.Clear();
            // initialize with current tick
            random = new Random();
        }

        private void LoadDefaultVariablesInternal()
        {
            LoadIntVariable("IMIN", int.MinValue);
            LoadIntVariable("IMAX", int.MaxValue);

            LoadFloatVariable("FMIN", double.MinValue);
            LoadFloatVariable("FMAX", double.MaxValue);
            LoadFloatVariable("FEPS", double.Epsilon);
            LoadFloatVariable("NAN", double.NaN);
            LoadFloatVariable("NINF", double.NegativeInfinity);
            LoadFloatVariable("PINF", double.PositiveInfinity);

            LoadFloatVariable("E", Math.E);
            LoadFloatVariable("PI", Math.PI);
        }

        private void LoadDefaultFunctionsInternal()
        {
            LoadFunction("print", LibPrint);
            LoadFunction("p", LibPrint);
            LoadFunction("cond", LibCond);
            LoadFunction("for", LibFor);
            LoadFunction("foreach", LibForEach);
            LoadFunction("size", LibSize);
            LoadFunction("shuffle", LibShuffle);

            LoadFunction("toi", LibToI);
            LoadFunction("tof", LibToF);
            LoadFunction("tos", LibToS);

            LoadFunction("abs", LibAbs);
            LoadFunction("exp", LibExp);
            LoadFunction("pow", LibPow);
            LoadFunction("sqrt", LibSqrt);
            LoadFunction("log", LibLog);
            LoadFunction("sin", LibSin);
            LoadFunction("cos", LibCos);
            LoadFunction("tan", LibTan);
            LoadFunction("asin", LibAsin);
            LoadFunction("acos", LibAcos);
            LoadFunction("atan", LibAtan);
            LoadFunction("atan2", LibAtan2);
            LoadFunction("rand", LibRand);
            LoadFunction("isprime", LibIsPrime);
        }

        private Value GetParam(Value[] args, int index)
        {
            if (index >= args.Length)
            {
                throw new RuntimeLangException(
                    $"Parameter #{index + 1} is required");
            }
            return args[index];
        }

        private void CheckIntRange(string name, int value, int min, int max)
        {
            if (value < min)
            {
                throw new RuntimeLangException($"{name} is less than {min}: {value}");
            }
            if (value > max)
            {
                throw new RuntimeLangException($"{name} is greater than {max}: {value}");
            }
        }

#region System functions
        private Value LibPrint(Value[] args)
        {
            bool first = true;
            foreach (var value in args)
            {
                if (!first)
                {
                    outputBuffer.Append(' ');
                }
                first = false;
                outputBuffer.Append(value.ToString());
            }
            outputBuffer.Append('\n');
            // max size = OutputSize
            outputBuffer.Length = Math.Min(outputBuffer.Length, OutputSize);

            return NilValue.Nil;
        }

        private Value LibCond(Value[] args)
        {
            bool b = GetParam(args, 0).ToBool();
            Value x = GetParam(args, 1);
            Value y = GetParam(args, 2);
            return b ? x : y;
        }

        private Value LibFor(Value[] args)
        {
            int start = GetParam(args, 0).ToInt();
            int end = GetParam(args, 1).ToInt();
            FunctionValue func = GetParam(args, 2).ToFunction();

            Value[] callArgs = new Value[1];
            for (int i = start; i <= end; i++)
            {
                callArgs[0] = new IntValue(i);
                CallFunction(func, callArgs);
            }

            return NilValue.Nil;
        }

        private Value LibForEach(Value[] args)
        {
            ArrayValue array = GetParam(args, 0).ToArray();
            List<Value> list = array.ValueList;
            FunctionValue func = GetParam(args, 1).ToFunction();

            Value[] callArgs = new Value[2];
            for (int i = 0; i < list.Count; i++)
            {
                callArgs[0] = list[i];
                callArgs[1] = new IntValue(i);
                CallFunction(func, callArgs);
            }

            return NilValue.Nil;
        }

        private Value LibSize(Value[] args)
        {
            ArrayValue array = GetParam(args, 0).ToArray();

            return new IntValue(array.ValueList.Count);
        }

        private Value LibShuffle(Value[] args)
        {
            ArrayValue array = GetParam(args, 0).ToArray();
            List<Value> target = array.ValueList;
            for (int i = target.Count - 1; i >= 1; i--)
            {
                int r = random.Next(i + 1);
                Value tmp = target[r];
                target[r] = target[i];
                target[i] = tmp;
            }

            return array;
        }
#endregion

#region Convert functions
        private Value LibToI(Value[] args)
        {
            Value x = GetParam(args, 0);
            return x.Type == ValueType.Float ?
                new IntValue((int)x.ToFloat()) :
                new IntValue(x.ToInt());
        }

        private Value LibToF(Value[] args)
        {
            Value x = GetParam(args, 0);
            return new FloatValue(x.ToFloat());
        }

        private Value LibToS(Value[] args)
        {
            Value x = GetParam(args, 0);
            return new StringValue(x.ToString());
        }
#endregion

#region Math functions
        private Value FloatFunc1(Value[] args, Func<double, double> func)
        {
            double x = GetParam(args, 0).ToFloat();
            return new FloatValue(func(x));
        }

        private Value FloatFunc2(Value[] args, Func<double, double, double> func)
        {
            double x = GetParam(args, 0).ToFloat();
            double y = GetParam(args, 1).ToFloat();
            return new FloatValue(func(x, y));
        }

        private Value LibAbs(Value[] args)
        {
            Value x = GetParam(args, 0);
            if (x.Type == ValueType.Int)
            {
                int i = x.ToInt();
                if (i == int.MinValue)
                {
                    throw new RuntimeLangException("Overflow at abs");
                }
                return new IntValue(Math.Abs(i));
            }
            else
            {
                double d = x.ToFloat();
                return new FloatValue(Math.Abs(d));
            }
        }

        private Value LibExp(Value[] args)
        {
            return FloatFunc1(args, Math.Exp);
        }

        private Value LibPow(Value[] args)
        {
            return FloatFunc2(args, Math.Pow);
        }

        private Value LibSqrt(Value[] args)
        {
            return FloatFunc1(args, Math.Sqrt);
        }

        private Value LibLog(Value[] args)
        {
            if (args.Length == 1)
            {
                return FloatFunc1(args, Math.Log);
            }
            else
            {
                return FloatFunc2(args, Math.Log);
            }
        }

        private Value LibSin(Value[] args)
        {
            return FloatFunc1(args, Math.Sin);
        }

        private Value LibCos(Value[] args)
        {
            return FloatFunc1(args, Math.Cos);
        }

        private Value LibTan(Value[] args)
        {
            return FloatFunc1(args, Math.Tan);
        }

        private Value LibAsin(Value[] args)
        {
            return FloatFunc1(args, Math.Asin);
        }

        private Value LibAcos(Value[] args)
        {
            return FloatFunc1(args, Math.Acos);
        }

        private Value LibAtan(Value[] args)
        {
            return FloatFunc1(args, Math.Atan);
        }

        private Value LibAtan2(Value[] args)
        {
            return FloatFunc2(args, Math.Atan2);
        }

        private Value LibRand(Value[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return new FloatValue(random.NextDouble());
                case 1:
                    {
                        int maxValue = GetParam(args, 0).ToInt();
                        CheckIntRange("MaxValue", maxValue, 0, int.MaxValue);
                        return new IntValue(random.Next(maxValue));
                    }
                default:
                    {
                        int minValue = GetParam(args, 0).ToInt();
                        int maxValue = GetParam(args, 1).ToInt();
                        CheckIntRange("MaxValue", maxValue, minValue, int.MaxValue);
                        return new IntValue(random.Next(minValue, maxValue));
                    }
            }
        }

        private Value LibIsPrime(Value[] args)
        {
            long x = GetParam(args, 0).ToInt();
            if (x == 2)
            {
                return BoolValue.Of(true);
            }
            if (x <= 1 || x % 2 == 0)
            {
                return BoolValue.Of(false);
            }
            // 3 <= x <= 0x7fffffff
            for (long i = 2; i * i <= x; i++)
            {
                if (x % i == 0)
                {
                    return BoolValue.Of(false);
                }
            }
            return BoolValue.Of(true);
        }
#endregion
    }
}