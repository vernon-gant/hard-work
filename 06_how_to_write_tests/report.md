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

![4](https://github.com/user-attachments/assets/e87237c1-a0f8-42a7-aadc-4cb83199d0df)

## Train builder

This was my university project in the first year from the procedural programming course.  
Small enough but suits our purpose. Here are the requirements:

```
Console tool that lets the user build a small train carriage-by-carriage, list its make-up, and print simple statistics.

- Train consists of carriages. More precisely, it is an ordered sequence of at most ten carriages. Ordering starts with 0 and goes up to train.Count - 1 and is called "index". Train consists of max 10 carriages.
- A carriage has two attributes:
    - type which also produces a one letter identifier for printing.
        - Passenger - P
        - Sleeper - S
        - Diner - D
    - capacity – an integer from 20 to 130.
- Carriage is added to the train by the user. When we insert a new carriage into the train, we also provide an index for the carriage to be inserted at. Starting from the index (including the index itself), we move all carriages to the right. For example:

Train has 5 carriages inside:

[D,D,P,P,P]
 0,1,2,3,4

We insert a new sleeper carriage at index 3. So we move 3 and 4 carriages to the right.

[D,D,P,S,P,P]
 0,1,2,3,4,5

- If we insert at the 0th index, then we move all carriages to the right. We can also append to the end of the train by entering the train.Count index.
```

As said above, the design phase remains as it is.  
The design is quite simple here :)  
We manipulate with a [Train](https://github.com/vernon-gant/hard-work/blob/master/06_how_to_write_tests/TrainBuilder/src/ITrain.cs)  
and insert [carriages](https://github.com/vernon-gant/hard-work/blob/master/06_how_to_write_tests/TrainBuilder/src/ICarriage.cs) into it.  
So these are our ADTs, and we can also extract the insertion validation into a separate ADT with the help of FluentValidation and represent the insertion rules in code.  
That's basically it.

I started with implementing validation rules for insertion.  
I just defined my expectations in this BDD form proposed by ChatGPT — which I really liked — and started writing tests that failed, and then covering them with a green commit that passed the single test.  
I decided not to expand a test commit to multiple tests (even to 2) to keep it really simple and make micro commits.  
As said, for the [first commit (this is a squashed version with test and implementation)](https://github.com/vernon-gant/hard-work/commit/8d4158a70f8f5189043de51f2753e2a01db3505a) I simply added a test for not allowing negative indexes  
and then a rule for that in the validator.  
The key is that I did not hold in my head all subsequent rules such as that the insertion index must also be less or equal to the current train size.  
The next change, for example, was the [check if the train is full](https://github.com/vernon-gant/hard-work/commit/28ea2dcaf1dfbc05f41a43dd70fd392a31551866), where I simply added a new rule.  
[Later](https://github.com/vernon-gant/hard-work/commit/f42cd1833e241faddacc7d9e64093abe1da72e4a) I refactored the rule from the first commit and added a constraint for the max possible insertion index.  
So gradually I built my validator, and at the end, it covered all the requirements.  
Sometimes, after writing a test from my spec description, I figured out that the test was already green, so there was no need to implement anything, and I committed a green test right away.  
Train implementation followed the same approach.

In this case, the project was small and more an educative one, so I could really commit a few lines of code without any problems.  
I will try to do this at my job later, but I anticipate that it is realistic to follow this idea because I really like it :)  
A few times, I made a rollback to the test commit after implementing validation rules incorrectly.  
Code disappeared, but it was not that much — 5 or 10 lines is nothing.  
Could I commit less than a few lines of code? Probably not :)  
But I guess because the complexity of this project was not that high, and I could come up with good specs, I could do it pretty well.  
I think the only difference in real projects will be time — probably it will take just a bit more time to come up with tests,  
but I am pretty sure that I will be able to remain on this micro commit layer.

## Encode decode

I was also interested in whether this approach is applicable to algorithmic tasks, like those from LeetCode. But not for one-function tasks — rather, at least tasks with two methods. So I chose this task:

```
Given an array of strings s[], you are required to create an algorithm in the encode() function that can convert the given strings into a single encoded string, which can be transmitted over the network and then decoded back into the original array of strings. The decoding will happen in the decode() function.

You need to implement two functions:
1. encode(): This takes an array of strings s[] and encodes it into a single string.
2. decode(): This takes the encoded string as input and returns an array of strings containing the original array as given in the encode method.

Note: You are not allowed to use any inbuilt serialize method.

Input: s = ["Hello","World"]
Output: ["Hello","World"]
Explanation: The encode() function will have the str as input, it will be encoded by one machine. Then another machine will receive the encoded string as the input parameter and then will decode it to its original form.

Input: s = ["abc","!@"]
Output: ["abc","!@"]
Explanation: The encode() function will have the str as input, here there are two strings, one is "abc" and the other one has some special characters. It will be encoded by one machine. Then another machine will receive the encoded string as the input parameter and then will decode it to its original form.

Constraints:
1<=s.size()<=100
1<=s[i].size()<=100
s[i] contains any possible characters out of the 256 ASCII characters.
```

I started implementing it using different test cases.
For example, I started with the [empty list](https://github.com/vernon-gant/hard-work/commit/03a2b98ec83454d4399538e2dc2e8aca1da24c66), followed by a list [with an empty string](https://github.com/vernon-gant/hard-work/commit/d082096da75c584cdf0ea9f507187f00ff234e36).
Yes, this improved my understanding of the problem because already on the second commit I had to figure out how to differentiate between an empty list and a list with an empty string, and the same default implementation did not work.
Then I added the first list with multiple words and implemented the [algorithm](https://github.com/vernon-gant/hard-work/commit/d082096da75c584cdf0ea9f507187f00ff234e36).

However, after that, my implementation did not change at all and I just added more tests for special characters:

```c#
yield return new TestCaseData(new List<string> { "\u0001\u0002\u0003", "\u0004\u0005" });
yield return new TestCaseData(new List<string> { "áéíóú", "ßçü" });
yield return new TestCaseData(new List<string> { "!@#$%^&*()", "<>[]{}" });
yield return new TestCaseData(new List<string> { "!@#$%^&*()_;:'\"|\\]|\\||-" });
yield return new TestCaseData(new List<string> { "!@#$%", "^&*()", "_;:'\"", "|\\]|\\||-" });
yield return new TestCaseData(new List<string> { "hello!@#", "world%^&", "test_|:" });
```

I would not say that this approach is the best for pure algorithmic tasks — especially not for logic-heavy or domain-related tasks.
Nothing to say about one-function tasks like sliding window or dynamic programming challenges either.
There was also a big jump between the mock implementation and the correct algorithm, and I could not split these changes into smaller ones without breaking the logic. Lesson learned anyway!
