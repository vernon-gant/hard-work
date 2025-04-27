## Introduction

From the beginning, this idea to minimize changes and commit on every green test looked very attractive.  
However, my first question was — what do I do with design? How should I break the design into multiple micro steps? And what if some approach does not perform that well?  
However, after some research and a small conversation with ChatGPT, I started getting the idea.  
I just mixed up the 2nd and 3rd levels.

Indeed, when we talk about micro changes and micro commits, we always talk about implementation, and it is the second level.  
Design remains on the 3rd level, and this approach barely touches it.  
The main idea is that we really just "minimize the amount of changes we keep in our head when implementing a certain functionality."  
Now I understand that I was doing everything wrong.  
And when we understand this idea of micro commit TDD, BDD comes naturally.  
Yes, I admit I was not digging that deep into that topic; ChatGPT just proposed the style of writing tests in the "Given, When, Then," and this is really amazing.  
We simply put focus on behaviors, and this makes much more sense than what I was doing before — just testing the implementation details.  
But now back to the micro commit approach.  
So design remains the number 1 step. Without — I do not want to say "properly" — but just designed ADTs, you cannot start writing tests.

So firstly, we do probably preliminary design, and then the core idea of the micro step approach is that when implementing a certain ADT, we follow our list of expectations which we have written down — this is important, so that we later do not "fix" tests because the expectation was wrong or some domain concept had a misleading understanding.  
This was my mistake in the "TrainBuilder" — I messed up some tests because of misunderstanding how a new carriage is inserted.  
After some time, I had to rewrite tests, and thus the whole branch was in an invalid state the whole time before fixing.  
Okay, not a big deal, but still it is better to spend time at the beginning setting up the specification properly and then taking each of them one by one and implementing it.

I like the idea of minimizing time between specification and implementation.  
We simply want a certain functionality to start working + do not break existing ones.  
So at every point in time, we are concentrated only on one feature.  
This reduces the load as meditation does :)  
The only issue I was thinking about — what if, say, after 10 tests we come up that the whole method must be rewritten?  
And here is the answer — a good designed ADT must not do 1 billion things at a time.  
30-40 lines of code, where 40 seems already a lot, do not seem a big deal to delete and reimplement.  
All roads lead to a good design — as always.

If we convert it to the applied understanding, we come to a simple strategy:

1. Design your program/part of it.
2. Choose a module and write specifications in BDD or any format.
3. Put your ADTs in code in interfaces/abstract classes.
4. Come up with some sort of implementation (because in order to know how to do the task we must firstly complete it :).
5. Take the specification and write a red test case. Commit it, so that if our implementation is wrong we can simply rollback to this commit and start implementing from scratch.
6. Make the test pass with the green commit.
7. Repeat for all specifications.
8. Do interactive rebase and squash test and feature commits to produce one commit (optionally).

Important here is that we never push when our branch is in a red state — when the last commit is a red test commit.  
We only push after, say, rebasing and squashing commits.  
To make it easier, we can use the same name for spec description with just two different markers for test and feature commits, which are then squashed.  
It is easier to identify them because they come one after another and have the same description. Like these:

```
test: inserting into empty train adds single carriage
feat: inserting into empty train adds single carriage
```

Finally, this leads us to a balanced and pretty commit history.

// 4