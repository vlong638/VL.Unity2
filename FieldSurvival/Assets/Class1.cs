class Bank
{
    public void BankLoanFor16Year()
    {
        new WorkingStaffA().DoSomething();
        new WorkingStaffB().DoSomething();
        new WorkingStaffC().DoSomething();
    }
    public void BankLoanFor17Year()
    {
        new WorkingStaffA().DoSomething();
        new WorkingStaffB().DoSomething();
        new WorkingStaffC().DoSomething();
        new WorkingStaffD().DoSomething();
    }
}

class WorkingStaffBase
{
    public void DoSomething()
    {

    }
}
class WorkingStaffA: WorkingStaffBase
{
}
class WorkingStaffB : WorkingStaffBase
{
}
class WorkingStaffC : WorkingStaffBase
{
}
class WorkingStaffD : WorkingStaffBase
{
}

