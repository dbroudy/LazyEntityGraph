# LazyEntityGraph
LazyEntityGraph is the successor to [AutoEntityFramework](//github.com/alexfoxgill/AutoFixture.AutoEntityFramework). This project aims to both improve upon the functionality of the original, and open the door to integration with other test object creators and ORMs.

### What's New?
The key benefits of LazyEntityGraph over AutoEntityFramework are:

- **Extensible architecture**: AutoFixture and Entity Framework integration is available through extensions; similarly it would be possible to create extensions for NHibernate, ObjectHydrator, etc
- **Constrained relationships**: Specify one-to-one, one-to-many or many-to-many relationships between entities and the generation process will respect those relationships
- **Entity Framework integration**: Automatically generate model metadata from EDMX or Code First DbContexts, including foreign keys

[For documentation, see this wiki page](//github.com/dbroudy/LazyEntityGraph/wiki)

## Objective
When writing tests, we often need to create test objects without necessarily caring about the data used to populate them. There are many projects which accomplish this in .NET, among them [AutoFixture](https://github.com/AutoFixture/AutoFixture), [ObjectHydrator](https://github.com/PrintsCharming/ObjectHydrator), [NBuilder](https://github.com/garethdown44/nbuilder) and more.

Also popular in the .NET world are ORMs: libraries used to bridge the gap between data and code by representing relational data as C# objects. The most popular of these are [Entity Framework](https://docs.microsoft.com/en-us/ef/) and [NHibernate](https://github.com/nhibernate/nhibernate-core). These produce graphs of classes linked by relationship properties. 

```CSharp
public class Order
{
  public int Id { get; set; }
  public virtual ICollection<Product> Products { get; set; }
}

public class Product
{
  public int Id { get; set; }
  public virtual ICollection<Order> Orders { get; set; }
}
```

However, if you are using a test object creation library like AutoFixture, you will run into a problem when trying to create an instance of the `Order` class above: the circular reference between `Product` and `Order`. 

When using the ORM to retrieve one of these objects, you typically don't want to retrieve the entire object graph...! For this reason, relationship properties are often marked `virtual`, to enable them to be lazy-loaded.

So, we come to the purpose of this library: **to permit test object creation tools like AutoFixture to create entity graphs with circular references**.
