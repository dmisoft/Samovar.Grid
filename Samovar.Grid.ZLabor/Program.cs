using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.IO.Pipes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;
using Unity;

namespace Samovar.Grid.ZLabor
{
    class Program
    {
        static void Main(string[] args)
        {
            Mitarbeiter mainMA = new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" };
            Mitarbeiter refMA = mainMA;
            refMA.Name = "change";
            refMA = null;
            List<Setting> columns = new List<Setting>();
            columns.Add(new Setting<DataColumnModel> { Name = "data", Value = new DataColumnModel()});
            columns.Add(new Setting<DataColumnModel> { Name = "data", Value = new DataColumnModel() });
            columns.Add(new Setting<CommandColumnModel> { Name = "command", Value = new CommandColumnModel() });

            IEnumerable<Setting<DataColumnModel>> dataModels = columns.OfType<Setting<DataColumnModel>>();// . Where(s => s.Name.Equals("data"));
            IEnumerable<Setting<CommandColumnModel>> commandModels = columns.OfType<Setting<CommandColumnModel>>();// . Where(s => s.Name.Equals("data"));


            List<IColumnModel> columns1 = new List<IColumnModel>();
            columns1.Add(new DataColumnModel { Field = "Data", Order = 1});
            columns1.Add(new DataColumnModel { Field = "Caption", Order = 2});
            columns1.Add(new CommandColumnModel { Command = "Edit", Order = 3});

            IEnumerable<DataColumnModel> dataModels1 = columns1.OfType<DataColumnModel>();
            IEnumerable<CommandColumnModel> commandModels1 = columns1.OfType<CommandColumnModel>();

            var container = new UnityContainer();
            container.RegisterType<ICar, BMW>();
            container.RegisterType<ICar, Audi>();
            container.RegisterType<ICar, Ford>();

            var driver = container.Resolve<Driver>();
            driver.RunCar();

            Console.ReadLine();
            return;
            RXTest rxTest = new RXTest();
            rxTest.Start();

            Console.ReadLine();
            var c = Channel.CreateUnbounded<int>();

            //_ = Task.Run(async delegate
            //{
            //    for (int i = 0; ; i++)
            //    {
            //        await Task.Delay(1000);
            //        await c.Writer.WriteAsync(i);
            //    }
            //});

            //while (true) {
            //    Console.WriteLine(await c.Reader.ReadAsync());
            //}

            //var ch = Channel.CreateUnbounded<Mitarbeiter>();

            //var consumer = Task.Run(async () =>
            //{
            //    while (await ch.Reader.WaitToReadAsync())
            //        Console.WriteLine(((await ch.Reader.ReadAsync()) as Mitarbeiter).Name);
            //});
            //var producer = Task.Run(async () =>
            //{
            //    var rnd = new Random();
            //    for (int i = 0; i < 5; i++)
            //    {
            //        await Task.Delay(TimeSpan.FromSeconds(rnd.Next(3)));
            //        var m = new Mitarbeiter { 
            //        Abteilung=100, Name = "Name", Vorname ="Vorname", Team = "Team"
            //        };

            //        await ch.Writer.WriteAsync(m);
            //    }
            //    ch.Writer.Complete();
            //});

            //await Task.WhenAll(producer, consumer);


            return;
            IEnumerableExtTest();
            Console.ReadLine();
            DynLinqTest();
            StdGroup();

            BindingListTest_2();
            BindingListTest_1();
            DateTime dt = DateTime.Now;
            string str = dt.ToString("dd . MM . yy");
            int? num1 = null;
            int? num2 = null;
            if (num1 > num2) { }
            LambdaTest();
            TestAusDll();


            //CreateDict();
            //object mitarbeiterList;
            //List<Mitarbeiter> mitarbeiterListRaw = new List<Mitarbeiter>();
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            //mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });

            //List<Mitarbeiter> list = Convert. ChangeType(mitarbeiterListRaw, typeof(List<Mitarbeiter>));

            //ConstantExpression valueExp = Expression.Constant(filterCellInfo.FilterCellValue.ToString().ToLower());
            //MemberExpression memberExp = Expression.Property(Expression.Convert(pairValueExp, t.GetProperty(field).DeclaringType), field);

            //MemberExpression memberExp = Expression.Property(Expression.Convert(pairValueExp, t.GetProperty(field).DeclaringType), field);
            //MethodInfo convertMethod = typeof(object).GetMethod("Convert", Type.EmptyTypes);
            //var callConvertMethodExp = Expression.Call(memberExp, propertyToLowerMethod);


            //    Type t = mitarbeiterList.ElementAt(0).GetType();
            //    var source = Expression.Parameter(t, "o");

            //    //var la = DynamicExpressionParser.ParseLambda(t, "new (Abteilung, Team)", new ParameterExpression[] { source });
            //    var grrr = mitarbeiterList.AsQueryable().GroupBy("new (Abteilung, Team) as Mitarbeiter", new ParameterExpression[] { source });
            //    //var grrr = mitarbeiterList.GroupBy(m => new { m.Abteilung, m.Team});
            //    foreach (var namegroup in grrr)
            //    {
            //        //Console.WriteLine($"key: {namegroup.Key}");
            //        //foreach (var mitarbeiter in namegroup)
            //        //{
            //        //    Console.WriteLine($"\t{mitarbeiter.Name}, {mitarbeiter.Vorname}");
            //        //}
            //    }
            //    var an = new { Abteilung = 1, Team = "" };
            //    var fieldsToGroupBy = new string[] { "Abteilung", "Team" };
            //    var bind1 = Expression.Bind(t.GetProperty("Abteilung"), Expression.Property(source, "Abteilung"));
            //    var bind2 = Expression.Bind(t.GetProperty("Team"), Expression.Property(source, "Team"));
            //    var mm = Expression.MemberInit(Expression.New(t), new MemberBinding[] { bind1, bind2 });
            //    //var rrrr = DynamicFields(typeof(Mitarbeiter), fieldsToGroupBy);
            //    var lambda11 = Expression.Lambda<Func<dynamic,dynamic>>(mm, new ParameterExpression[] { source });

            //    var grouped = mitarbeiterList.AsQueryable().GroupBy(lambda11);


            //    //var r = (dynamic)grouped;

            //    ParameterExpression x = Expression.Parameter(t,"p");
            //    List<Mitarbeiter> list = (List<Mitarbeiter>)Convert.ChangeType(mitarbeiterList, typeof(List<Mitarbeiter>));
            //    //var resGroup = mitarbeiterList.AsQueryable().GroupBy("new (Abteilung, Team)", new ParameterExpression[] { x });

            //    //for (int i = 0; i < resGroup.Count(); i++)
            //    //{
            //    //    //var expectedRow = result[i];

            //    //    // The DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
            //    //    var testRow = (IGrouping<DynamicClass, Mitarbeiter>)resGroup.ToDynamicListAsync().Result[i];

            //    //    Console.WriteLine("Key.BlogId   = {0}", ((dynamic)testRow.Key).BlogId);
            //    //    Console.WriteLine("Key.PostDate = {0}", ((dynamic)testRow.Key).PostDate);
            //    //    Console.WriteLine("PostTitles   = {0}", string.Join(", ", testRow.Select(x => $"'{x.Name}'")));
            //    //    Console.WriteLine();
            //    //}
            //    ////var resGroup2 = from m in mitarbeiterList group m by (m.Abteilung, m.Team) into gr select gr;
            //    ////foreach (var nameGroup in resGroup2)
            //    ////{
            //    ////    Console.WriteLine($"Key: {nameGroup.Key}");
            //    ////    foreach (var mitarbeiter in nameGroup)
            //    ////    {
            //    ////        Console.WriteLine($"\t{mitarbeiter.Name}, {mitarbeiter.Vorname}");
            //    ////    }
            //    ////}



            //    ////Type type = Repository.DataType;
            //    ////foreach (PropertyInfo pi in type.GetProperties())
            //    ////{
            //    ////    PropInfo.Add(pi.Name, pi);
            //    ////}
            //    //List<PropertyInfo> pl = new List<PropertyInfo> { typeof(Mitarbeiter).GetProperty("Abteilung"), typeof(Mitarbeiter).GetProperty("Team") };
            //    //Func<Mitarbeiter, (string, string)> grSelector = sel => (sel.Abteilung, sel.Team);//  new Func<Mitarbeiter, (string, string)>(m => m.Abteilung);
            //    //var resGroup3 = from m in mitarbeiterList group m by grSelector into gr3 select gr3;
            //    //foreach (var nameGroup in resGroup3)
            //    //{
            //    //    Console.WriteLine($"Key: {nameGroup.Key}");
            //    //    foreach (Mitarbeiter mitarbeiter in nameGroup)
            //    //    {
            //    //        Console.WriteLine($"\t{mitarbeiter.Name}, {mitarbeiter.Vorname}");
            //    //    }
            //    //}

            //    ////var gr1 = mitarbeiterList.GroupBy(m => m.Name);

            //    ////var usersGroupedByCountry = mitarbeiterList.GroupBy(grSelector);
            //    //var result = mitarbeiterList.AsQueryable().GroupBy("(Abteilung as string,Team as string)");

            //    //for (int i = 0; i < result.Count(); i++)
            //    //{
            //    //    //var expectedRow = result[i];

            //    //    // The DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
            //    //    var testRow = (IGrouping<DynamicClass, Mitarbeiter>)result.ToDynamicListAsync().Result[i];

            //    //    Console.WriteLine("Key.BlogId   = {0}", ((dynamic)testRow.Key).BlogId);
            //    //    Console.WriteLine("Key.PostDate = {0}", ((dynamic)testRow.Key).PostDate);
            //    //    Console.WriteLine("PostTitles   = {0}", string.Join(", ", testRow.Select(x => $"'{x.Name}'")));
            //    //    Console.WriteLine();
            //    //}
            //}

            //private static void CreateDict()
            //{
            //    string tree1 = "maple";
            //    string tree2 = "oak";

            //    System.Reflection.MethodInfo addMethod = typeof(Dictionary<int, string>).GetMethod("Add");

            //    // Create two ElementInit objects that represent the
            //    // two key-value pairs to add to the Dictionary.
            //    System.Linq.Expressions.ElementInit elementInit1 =
            //        System.Linq.Expressions.Expression.ElementInit(
            //            addMethod,
            //            System.Linq.Expressions.Expression.Constant(tree1.Length),
            //            System.Linq.Expressions.Expression.Constant(tree1));
            //    System.Linq.Expressions.ElementInit elementInit2 =
            //        System.Linq.Expressions.Expression.ElementInit(
            //            addMethod,
            //            System.Linq.Expressions.Expression.Constant(tree2.Length),
            //            System.Linq.Expressions.Expression.Constant(tree2));

            //    // Create a NewExpression that represents constructing
            //    // a new instance of Dictionary<int, string>.
            //    System.Linq.Expressions.NewExpression newDictionaryExpression =
            //        System.Linq.Expressions.Expression.New(typeof(Dictionary<int, string>));

            //    // Create a ListInitExpression that represents initializing
            //    // a new Dictionary<> instance with two key-value pairs.
            //    System.Linq.Expressions.ListInitExpression listInitExpression =
            //        System.Linq.Expressions.Expression.ListInit(
            //            newDictionaryExpression,
            //            elementInit1,
            //            elementInit2);

            //    Console.WriteLine(listInitExpression.ToString());
            //    var ret = Expression.Lambda<Func<object>>(listInitExpression, null);

        }

        private static void IEnumerableExtTest()
        {

            List<Mitarbeiter> mitarbeiterListRaw = new List<Mitarbeiter> {
            new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" },
            new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" },
            new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" },
            new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" },
            new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" },
            new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" },
            new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" },
            new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" }
            };

            mitarbeiterListRaw[2] = new Mitarbeiter { Name = "Name 33", Vorname = "Vorname 33", Abteilung = 33, Team = "Team 313" };

            var col = mitarbeiterListRaw.ToObservableCollection();
            col.CollectionChanged += Program_CollectionChanged;
        }

        private static void Program_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private static void DynLinqTest()
        {
            object mitarbeiterList;
            List<Mitarbeiter> mitarbeiterListRaw = new List<Mitarbeiter>();
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            mitarbeiterListRaw.Add(new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });

            mitarbeiterList = mitarbeiterListRaw;

            //Type t = mitarbeiterList.GetType();
            //var source = Expression.Parameter(t, "o");

            //var typeAsExpression = Expression.TypeAs(source, t);

            //var asQueryableExpression = Expression.Call(typeAsExpression, typeof(Queryable).GetMethod("GroupBy", new[] { typeof(string) }));
            //var groupByExpression = Expression.Call(asQueryableExpression, t.GetMethod("GroupBy"), Expression.Constant("new (Abteilung, Team)"));
            //var lambda = Expression.Lambda(groupByExpression, new ParameterExpression[] { source });


            var result = mitarbeiterListRaw.AsQueryable().GroupBy("new (Abteilung, Team)").ToDynamicArray();

            for (int i = 0; i < result.Length; i++)
            {
                var expectedRow = result[i];

                // The DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DynamicClass, Mitarbeiter>)result[i];

                Console.WriteLine("Key.Abteilung   = {0}", ((dynamic)testRow.Key).Abteilung);
                Console.WriteLine("Key.Team = {0}", ((dynamic)testRow.Key).Team);
                Console.WriteLine("Mitarbeiter   = {0}", string.Join(", ", testRow.Select(x => $"'{x.Name}, {x.Vorname}'")));
                Console.WriteLine();
            }
        }

        static void LambdaTest()
        {
            List<string> groupingColumns = new List<string>();
            groupingColumns.Add("Abteilung");

            Dictionary<Guid, Mitarbeiter> DataDictionary = new Dictionary<Guid, Mitarbeiter>();
            //List<object> mitarbeiterList = new List<object>();
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            DataDictionary.Add(Guid.NewGuid(), new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });

            Type t = DataDictionary.ElementAt(0).Value.GetType();
            ParameterExpression param = Expression.Parameter(typeof(KeyValuePair<Guid, Mitarbeiter>), "pair");
            MemberExpression pairValueExp = Expression.Property(param, "Value");
            MemberExpression body1 = Expression.Property(Expression.Convert(pairValueExp, t.GetProperty("Abteilung").DeclaringType), "Abteilung");
            MemberExpression body2 = Expression.Property(Expression.Convert(pairValueExp, t.GetProperty("Team").DeclaringType), "Team");


            //var body1 = Expression.Property(param, "Abteilung");
            //var body2 = Expression.Property(param, "Team");
            var body = Expression.Block(new Expression[] { body1, body2 });

            var anonymousType = new { Abteilung = 0, Team = "" }.GetType();

            var parameter = Expression.Parameter(typeof(Mitarbeiter), "person");

            var selector = Expression.New(
                   anonymousType.GetConstructor(new[] { typeof(int), typeof(string) }),
                   Expression.Property(parameter, "Abteilung"),
                   Expression.Property(parameter, "Team"));

            var selector2 = Expression.New(typeof(Mitarbeiter).GetConstructors()[0], Expression.Property(parameter, "Abteilung"),
                   Expression.Property(parameter, "Team"));

            var lambda1 = Expression.Lambda<Func<KeyValuePair<Guid, Mitarbeiter>, object>>(selector, new ParameterExpression[] { param });

            var resGroup = DataDictionary.AsQueryable().GroupBy(lambda1);
            foreach (var nameGroup in resGroup)
            {
                System.Diagnostics.Debug.WriteLine($"Key: {nameGroup.Key}");
                foreach (var mitarbeiter in nameGroup)
                {
                    System.Diagnostics.Debug.WriteLine($"       {mitarbeiter.ToString()}");
                }
            }

            //var la = Expression.Lambda<Func<Mitarbeiter, dynamic>>(body, param);
            //List<Expression> lambdaList = new List<Expression>();

            //foreach (var item in groupingColumns)
            //{
            //    MemberExpression pairValueExp = Expression.Property(param, "Value");
            //    MemberExpression propertyExp = Expression.Property(Expression.Convert(pairValueExp, t.GetProperty(item).DeclaringType), item);
            //    lambdaList.Add(propertyExp);

            //    //Expression.New()
            //    var l = Expression.Lambda<Func<KeyValuePair<Guid, object>, object>>(propertyExp, new ParameterExpression[] { param });
            //}

            //Expression retLambda = null;
            //if (lambdaList.Count() == 1)
            //{
            //    retLambda = lambdaList[0];
            //}
            //else if (lambdaList.Count() >= 2)
            //{
            //    retLambda = Expression.AndAlso(lambdaList[0], lambdaList[1]);
            //    for (int i = 2; i < lambdaList.Count(); i++)
            //    {
            //        retLambda = Expression.AndAlso(retLambda, lambdaList[i]);
            //    }
            //}

            //try
            //{
            //    var lambda = Expression.Lambda<Func<KeyValuePair<Guid, object>, object>>(retLambda, new ParameterExpression[] { param });
            //    var resGroup = DataDictionary.AsQueryable().GroupBy(lambda);

            //    foreach (var nameGroup in resGroup)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"Key: {nameGroup.Key}");
            //        foreach (var mitarbeiter in nameGroup)
            //        {
            //            System.Diagnostics.Debug.WriteLine($"       {mitarbeiter.ToString()}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}


        }

        private static void StdGroup()
        {
            List<Mitarbeiter> mitarbeiterList = new List<Mitarbeiter>();
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });


            var gr = mitarbeiterList.AsQueryable().GroupBy(m => new { m.Abteilung, m.Team });
            foreach (var namegroup in gr)
            {
                Debug.WriteLine($"key: {namegroup.Key}");
                foreach (var mitarbeiter in namegroup)
                {
                    Debug.WriteLine($"\t{mitarbeiter.Name}, {mitarbeiter.Vorname}");
                }
            }

        }

        private static void TestAusDll()
        {
            //ClassTest cl = new ClassTest();
            //cl.Test();
        }
        //public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, params object[] values)
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (keySelector == null) throw new ArgumentNullException("keySelector");
        //    if (elementSelector == null) throw new ArgumentNullException("elementSelector");
        //    LambdaExpression keyLambda = DynamicExpressionParser.ParseLambda(source.ElementType, null, keySelector, values);
        //    LambdaExpression elementLambda = DynamicExpressionParser.ParseLambda(source.ElementType, null, elementSelector, values);
        //    return source.Provider.CreateQuery(
        //        Expression.Call(
        //           typeof(Queryable), "GroupBy",
        //           new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
        //           source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        //}

        //static Expression<Func<TSource, dynamic>> DynamicFields<TSource>(IEnumerable<string> fields)
        //{
        //    var source = Expression.Parameter(typeof(TSource), "o");
        //    var properties = fields
        //        .Select(f => typeof(TSource).GetProperty(f))
        //        .Select(p => new DynamicProperty(p.Name, p.PropertyType))
        //        .ToList();
        //    var resultType = DynamicClassFactory.CreateType(properties, false);
        //    var bindings = properties.Select(p => Expression.Bind(resultType.GetProperty(p.Name), Expression.Property(source, p.Name)));
        //    var result = Expression.MemberInit(Expression.New(resultType), bindings);
        //    return Expression.Lambda<Func<TSource, dynamic>>(result, source);
        //}

        //static Expression<Func<dynamic>> DynamicFields(Type t, IEnumerable<string> fields)
        //{
        //    var source = Expression.Parameter(t, "o");
        //    var properties = fields
        //        .Select(f => t.GetProperty(f))
        //        .Select(p => new DynamicProperty(p.Name, p.PropertyType))
        //        .ToList();
        //    var resultType = DynamicClassFactory.CreateType(properties, false);
        //    var bindings = properties.Select(p => Expression.Bind(resultType.GetProperty(p.Name), Expression.Property(source, p.Name)));
        //    var result = Expression.MemberInit(Expression.New(resultType), bindings);
        //    return Expression.Lambda<Func<dynamic>>(result, source);
        //}

        private static void BindingListTest_1()
        {
            BindingList<Mitarbeiter> mitarbeiterList = new BindingList<Mitarbeiter>();
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });
            mitarbeiterList.ListChanged += MitarbeiterList_ListChanged;
            Console.WriteLine($"Hashcode: {mitarbeiterList.GetHashCode()}");
            mitarbeiterList[0] = new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" };
            Console.WriteLine($"Hashcode: {mitarbeiterList.GetHashCode()}");
        }

        private static void BindingListTest_2()
        {
            List<Mitarbeiter> mitarbeiterList = new List<Mitarbeiter>();
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });

            BindingList<Mitarbeiter> bl = new BindingList<Mitarbeiter>(mitarbeiterList);
            bl.ListChanged += MitarbeiterList_ListChanged;

            mitarbeiterList[0] = new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" };

        }

        private static void Oc_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private static void MitarbeiterList_ListChanged(object sender, ListChangedEventArgs e)
        {

        }
    }

    public interface IColumnModel
    {
        public int Order { get; set; }
    }

    public interface IDataColumnModel
    : IColumnModel
    {
        public string Field { get; set; }
    }

    public interface ICommandColumnModel
    : IColumnModel

    {
        public string Command { get; set; }
    }

    public class DataColumnModel
        : IDataColumnModel
    {
        public int Order { get; set; }
        public string Field { get; set; }
        public DataColumnModel()
        {

        }
    }

    public class CommandColumnModel
        : ICommandColumnModel
    {
        public int Order { get; set; }
        public string Command { get; set; }
    }


    public abstract class MyColumnModel<T> where T : IColumnModel
    {
        public abstract T ColumnModel { get; set; }
    }

    public class MyDataColumnModel
        : MyColumnModel<IDataColumnModel>
    {

        public override IDataColumnModel ColumnModel { get; set; }
    }
    public class MyCommandColumnModel
        : MyColumnModel<ICommandColumnModel>
    {

        public override ICommandColumnModel ColumnModel { get; set; }
    }

    public class ColumnBase<T>
    {
        T Model { get; set; }
    }

    public class DataColumn
        : ColumnBase<DataColumnModel>
    {

    }

    class CommandColumn
        : ColumnBase<CommandColumnModel>
    {

    }

    public interface IMetadata
    {
        Type DataType { get; }

        object Data { get; }
    }

    public interface IMetadata<TData> : IMetadata
    {
        new TData Data { get; }
    }

    public class Metadata<TData> : IMetadata<TData>
    {
        public Metadata(TData data)
        {
            Data = data;
        }

        public Type DataType
        {
            get { return typeof(TData); }
        }

        object IMetadata.Data
        {
            get { return Data; }
        }

        public TData Data { get; private set; }
    }

    public interface IValueTypeMetadata : IMetadata
    {

    }

    public interface IValueTypeMetadata<TData> : IMetadata<TData>, IValueTypeMetadata where TData : struct
    {

    }

    public class ValueTypeMetadata<TData> : Metadata<TData>, IValueTypeMetadata<TData> where TData : struct
    {
        public ValueTypeMetadata(TData data) : base(data)
        { }
    }

    public class Setting
    {
        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {

                    throw new ApplicationException("Property [Name] in class [Setting] is null or empty");
                }
                return _name;
            }
            set { _name = value; }
        }
    }

[Serializable]    public class Setting<T> : Setting
    {
        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Define a default constructor for serialization purposes
        /// </summary>
        public Setting()
        {
        }
    }
}
