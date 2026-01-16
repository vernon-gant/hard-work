# How to treat TDD correctly

It indeed makes sense to refer back to previous notes after some time as it gives valuable insights. After rereading my notes to "thinking on the design level" and article about "How to treat TDD correctly" I can see the shift in my head on how I understand TDD.

Currently I understand TDD more as a mixture of a paradigm for code writing(but not design) and an effective testing technique. Why so? Firstly, as it was already mentioned in the article TDD influences local design decisions aka programming in small whereas programming in large requires more thinking on the logical level and completely different approach, in my opinion. Here we deal with more abstract system specification like in the example with a service which reads from 3 streams and saves something in a database. Although maybe not the best example for a design on a large level, it still requires more thinking on the third level, as we are not concerned about the JSON APIs we call or some database details. TDD can not really help here a lot.

Here we come back to the methodology of Betrantd Meyer with ADTs when we are in the OOP world. Now I see that the key is exactly in combining these two approachs and not just strictly and blindly following some idea. We can design the system/module using our abstract data types, then start crafting our classes/interfaces on the second level and here TDD will shine! Because as it was said we will be forced to think about the usage of our APIs, how it is tested, about the use cases - what our module should do. And this can be expressed using specifications in form of tests. Here the red-green code writing model really brings a lot of value. For every concrete module we try to come up with input/output combinations and then implement the behavior.

However even in this case as it was mentioned TDD does not cover everything. These are just module/unit tests. They are still a good regression base for our future refactorings but they do not check how various components of the system interact with each other. For that we use integration tests. An interesting idea would be, as the very first step to write one integration test and then for that use case develop modules we need and act again in the red green model until the integratin test passes. Sounds interesting.

However here as I think, again, we should not be driven by the test. The design should be more less ready. Single ADT specifications will be implemented using TDD, but not the general design.

What I find really good about TDD is that we are really forced to deeper think about the components logic during the implementation. We write more tests, what is always good. It does not gurantee us quality, but saves from many obvious errors.

## Summary

So, my current understanding of TDD aligns well with the provided article. It is not a silver bullet. It can give us a better design on a **local level**. It makes us write more tests and think about the code usage. And of course we have normally high code coverage because of the red green approach. We get also a strong regression base and can apply refactoring later easily.

However it does not cover all verification. E2E, integration tests, fuzzying if meaningful are other powerful techinques which can enrich our toolset. Type checks, asserts in code, manual testing also bring a lot of value. TDD gives us a lot on the local level, but programming in large requires definitely other tools and ideas, which I will grasp later :)