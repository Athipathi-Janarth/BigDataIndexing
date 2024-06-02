namespace BigDataIndexing.WebApi.Models;

public class Plan
{
    public PlanCostShares PlanCostShares { get; set; }
    public List<LinkedPlanService> LinkedPlanServices { get; set; }
    public string _org { get; set; }
    public string ObjectId { get; set; }
    public string ObjectType { get; set; }
    public string PlanType { get; set; }
    public DateTime CreationDate { get; set; }
}

public class PlanCostShares
{
    public int Deductible { get; set; }
    public string _org { get; set; }
    public int Copay { get; set; }
    public string ObjectId { get; set; }
    public string ObjectType { get; set; }
}

public class LinkedPlanService
{
    public LinkedService LinkedService { get; set; }
    public PlanserviceCostShares PlanserviceCostShares { get; set; }
    public string _org { get; set; }
    public string ObjectId { get; set; }
    public string ObjectType { get; set; }
}

public class LinkedService
{
    public string _org { get; set; }
    public string ObjectId { get; set; }
    public string ObjectType { get; set; }
    public string Name { get; set; }
}

public class PlanserviceCostShares
{
    public int Deductible { get; set; }
    public string _org { get; set; }
    public int Copay { get; set; }
    public string ObjectId { get; set; }
    public string ObjectType { get; set; }
}
