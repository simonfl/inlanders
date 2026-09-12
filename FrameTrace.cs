using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class Game
{
    private struct FrameTrace
    {
        public double PersonMovementMs;
        public double ActorSetupMs,PeopleMs,TreesMs,StockMs,BuildingsMs,SlowPersonMs;
        public int SlowPersonId,SlowPersonTask;
        // ProcessMs is elapsed callback time, not CPU residency. ThreadCpuMs is
        // Windows thread CPU over the whole frame (coarse OS tick resolution).
        public double WallMs,EngineDeltaMs,ThreadCpuMs,InputMs,SimulationMs,MaintenanceMs,ActorsMs,AtmosphereMs,FoodMs,HudMs,AudioMs,ProcessMs;
        public float SimulationTime;
        public long AllocatedBytes;
        public int Gen0,Gen1,Gen2,ActorMeshes,FoodMeshes,OtherMeshes,Ticks;
    }
    private bool _traceFrames;
    private readonly List<FrameTrace> _frameTraces=new();
    private FrameTrace _frameTrace;
    private ulong _traceStart,_traceMark,_traceActorMark;
    private long _traceAlloc;
    private double _traceThreadCpu;
    [DllImport("kernel32.dll")]
    private static extern bool GetThreadTimes(IntPtr thread,out long creation,out long exit,out long kernel,out long user);
    private static double TraceThreadCpuMs()=>OperatingSystem.IsWindows() && GetThreadTimes(new IntPtr(-2),out _,out _,out long kernel,out long user)?(kernel+user)/10000d:-1;
    private int _traceGen0,_traceGen1,_traceGen2,_traceMeshes;
    private void BeginFrameTrace(double delta)
    {
        if(!_traceFrames)return;
        ulong start=Time.GetTicksUsec();double cpu=TraceThreadCpuMs();
        if(_frameTraces.Count>0){var last=_frameTraces[^1];last.WallMs=(start-_traceStart)/1000d;last.EngineDeltaMs=delta*1000;last.ThreadCpuMs=cpu>=0?cpu-_traceThreadCpu:-1;
            last.AllocatedBytes=GC.GetAllocatedBytesForCurrentThread()-_traceAlloc;
            last.Gen0=GC.CollectionCount(0)-_traceGen0;last.Gen1=GC.CollectionCount(1)-_traceGen1;last.Gen2=GC.CollectionCount(2)-_traceGen2;_frameTraces[^1]=last;}
        _traceThreadCpu=cpu;
        _frameTrace=default;_traceMeshes=0;_traceStart=_traceMark=start;
        _traceAlloc=GC.GetAllocatedBytesForCurrentThread();_traceGen0=GC.CollectionCount(0);_traceGen1=GC.CollectionCount(1);_traceGen2=GC.CollectionCount(2);
    }
    private void TracePhase(int phase)
    {
        if(!_traceFrames)return;
        ulong now=Time.GetTicksUsec();double elapsed=(now-_traceMark)/1000d;_traceMark=now;
        switch(phase)
        {
            case 0:_frameTrace.InputMs=elapsed;break;
            case 1:_frameTrace.SimulationMs=elapsed;break;
            case 2:_frameTrace.MaintenanceMs=elapsed;_traceActorMark=now;break;
            case 3:_frameTrace.ActorsMs=elapsed;_frameTrace.ActorMeshes=_traceMeshes;_traceMeshes=0;break;
            case 4:_frameTrace.AtmosphereMs=elapsed;break;
            case 5:_frameTrace.FoodMs=elapsed;_frameTrace.FoodMeshes=_traceMeshes;_traceMeshes=0;break;
            case 6:_frameTrace.HudMs=elapsed;break;
            case 7:_frameTrace.AudioMs=elapsed;break;
        }
    }
    private void TraceActorPart(int part)
    {
        if(!_traceFrames)return;
        ulong now=Time.GetTicksUsec();double ms=(now-_traceActorMark)/1000d;_traceActorMark=now;
        switch(part){case 0:_frameTrace.ActorSetupMs=ms;break;case 1:_frameTrace.PeopleMs=ms;break;case 2:_frameTrace.TreesMs=ms;break;case 3:_frameTrace.StockMs=ms;break;case 4:_frameTrace.BuildingsMs=ms;break;}
    }
    private void TracePerson(int id,int task,ulong start)
    {
        if(!_traceFrames)return;
        double ms=(Time.GetTicksUsec()-start)/1000d;
        if(ms>_frameTrace.SlowPersonMs){_frameTrace.SlowPersonMs=ms;_frameTrace.SlowPersonId=id;_frameTrace.SlowPersonTask=task;}
    }
    private void EndFrameTrace()
    {
        if(!_traceFrames)return;
        _frameTrace.ProcessMs=(Time.GetTicksUsec()-_traceStart)/1000d;_frameTrace.OtherMeshes=_traceMeshes;_frameTrace.SimulationTime=_world.Food.Time;
        _frameTrace.AllocatedBytes=GC.GetAllocatedBytesForCurrentThread()-_traceAlloc;
        _frameTrace.Gen0=GC.CollectionCount(0)-_traceGen0;_frameTrace.Gen1=GC.CollectionCount(1)-_traceGen1;_frameTrace.Gen2=GC.CollectionCount(2)-_traceGen2;
        _frameTraces.Add(_frameTrace);
    }
}
