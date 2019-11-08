# NetForth
Forth on .NET

A version of Forth for .NET. Implemented as a single session object which can interpret Forth passed to it in strings.  The environment
stays the same during the life of the session object so names, definitions and stack all stay the same.  It would be relatively trivial
to write a simple Console app for a simple eval loop app (which is probably the last thing I'll do).

Features:
- Define any C# function as a primitive
- C# objects are represented as single objects on the stack and can be called into, constructed, modified, indexed, etc.
- ' (execute tokens) work fine
- List of supported words thus far (this list will probably go out of date quite rapidly with me making minor attempts to keep it up
to date)

// MATH
+
\-
*
/
um*
*/
mod
um/mod
negate
abs
min
max
1-
1+

// STACK
dup
?dup
drop
swap
over
nip
tuck
rot
-rot
pick
2dup
2drop
2swap
2over

// COMMENTS
\\
\(

// COMPARISONS
&lt;=
&gt;=
u&lt;
u&gt;
\=
&lt;&gt;
0=
0&lt;&gt;
0&gt;
0&lt;
&lt;
&gt;

// LOGICAL OPERATIONS
and
or
xor
invert
lshift
rshift

// DEFINING
:
constant
variable
value
'
&gt;does
execute

// MEMORY
@
!
c@
c!
w@
w!
b@
b!
+!
cells
chars

// FLOW OF CONTROL
if
do
?do
begin
i
j
leave
?leave
exit

// STRINGS
c\"
s\"
[char]
char
strhead
count
cstrTo.Net

// MEMORY ALLOTMENT
create
,
c,
here
allot

// RETURN STACK
&gt;r
r&gt;
r@
2&gt;r
2r&gt;
2r@

// I/O
.
.s
emit
cr
page
type
key
included

// .NET
defmeth
defcnst
defindx
defstat
prop
sprop
isnull
null

// Create a .net string and leave it's token on the stack
n"

// Create a type from the name and leave it's token on the stack. If it's a generic type then the types for it's generic arguments should be pushed on the stack ahead when the type is created.
t"

ToDo:
- Implement a floating point stack
- Make stack longs instead of ints?  Possibly as a compile switch.
- Make a compile switch that allows memory to be implemented slowly but safely.  Currently it's fast by being unsafe (though I do have
checks in the code that should keep everything safe).
- Setting indexed values and properties
- Probably some more standard Forth words I ought to implement
- Threads?
- File I/O
- Can I use IL code generation to speed things up a bit?
- Recursion
- More testing coverage

