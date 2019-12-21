namespace Tests

open Domain
open Operations
open Microsoft.VisualStudio.TestTools.UnitTesting

module Say =

    [<TestClass>]
    type TestClass () =

        [<TestMethod>]
        member this.TestIntRegexValid() =
            let matchResult = IntRegex.Match("123")
            Assert.IsTrue(matchResult.Success)

        [<TestMethod>]
        member this.TestIntRegexBad() =
            let matchResult = IntRegex.Match("123.")
            Assert.IsTrue(matchResult.Success = false)

        [<TestMethod>]
        member this.TestIntRegexValidNegative() =
            let matchResult = IntRegex.Match("-123")
            Assert.IsTrue(matchResult.Success)

        [<TestMethod>]
        member this.TestDollarStringToCentsValid1() =
            let result = DollarStringToCents("100")
            Assert.IsTrue(result.IsSome)
            Assert.AreEqual(10000, result.Value)

        [<TestMethod>]
        member this.TestDollarStringToCentsValid2() =
            let result = DollarStringToCents("100.1")
            Assert.IsTrue(result.IsSome)
            Assert.AreEqual(10010, result.Value)

        [<TestMethod>]
        member this.TestDollarStringToCentsValid3() =
            let result = DollarStringToCents("100.01")
            Assert.IsTrue(result.IsSome)
            Assert.AreEqual(10001, result.Value)

        [<TestMethod>]
        member this.TestDollarStringToCentsValid4() =
            let result = DollarStringToCents("100.10")
            Assert.IsTrue(result.IsSome)
            Assert.AreEqual(10010, result.Value)

        [<TestMethod>]
        member this.TestDollarStringToCentsValid5() =
            let result = DollarStringToCents("-100.10")
            Assert.IsTrue(result.IsSome)
            Assert.AreEqual(-10010, result.Value)
        
        [<TestMethod>]
        member this.TestDollarStringToCentsValid6() =
            let result = DollarStringToCents("-100")
            Assert.IsTrue(result.IsSome)
            Assert.AreEqual(-10000, result.Value)

        [<TestMethod>]
        member this.TestCentsToDollarString() =
            let result = CentsToDollarString(10010)
            Assert.AreEqual("100.10", result)

        [<TestMethod>]
        member this.TestCentsToDollarString2() =
            let result = CentsToDollarString(10000)
            Assert.AreEqual("100.00", result)

        [<TestMethod>]
        member this.TestCentsToDollarString3() =
            let result = CentsToDollarString(10001)
            Assert.AreEqual("100.01", result)

        [<TestMethod>]
        member this.TestCentsToDollarString4() =
            let result = CentsToDollarString(-10001)
            Assert.AreEqual("-100.01", result)

