# ConstrainedTypes
![.NET Core](https://github.com/aggieben/constrainedtypes/workflows/.NET%20Core/badge.svg)

One of the features I've often wanted in F♯ was an ability to more directly represent "sub-types" that are better tailored to some domain model than the built-in types.  In type theory this is called a [refinement type](https://en.wikipedia.org/wiki/Refinement_type).  For example, it's often the case that strings are stored in the database, but they are also just as often limited in length.  That tends to impose a constraint all across the domain, and so what I'd like to do in my domain model is capture that constraint so that invalid states are unrepresentable.

This package implements type providers that allow the user to essentially generate these refinement types at compile-time.  This won't go so far as provability systems will go, such as F* or Coq, but it will serve our domain modeling purposes.  I hesitate to call these types true refinement types because they don't do much to increase how much the compiler can do for you at compile time other than generate the types you need based on the specific types of constraints specified via the type provider mechanism.  It seems to me that true refinement types would require direct compiler support, which simply isn't present in F♯.  However, for the sake of domain modeling these types will get you most of the benefits of refinement types.

For example, if you have a 64-byte string in the database, one can effectively represent it thusly using the `BoundedString` type provider:

```fsharp
open ConstrainedTypes

type BoundedString64 = BoundedString<64>

type DomainModel =
    { Pk : uint64
      Name : BoundedString64 }
```

In this model, you can be sure that there is never a string longer than 64 characters stored under the to the `DomainModel.Name` label.  The `BoundedString` provider can be used to arbitrarily generate types for varying lengths, as required.

## Type Providers in this package:

### `BoundedString`

See example above

### `RangedInt`

Can be used like this:
```fsharp
open ConstrainedTypes

// this type can be used wherever you might have previously used an int.
type IntegersBetween0And100 = RangedInt<0,100>
```
