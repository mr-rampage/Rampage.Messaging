# Rampage.Messaging

A simple messaging framework for .NET applications. The goal was to provide a set of interfaces to build a simple message
bus as a mechanism to decouple highly-coupled code. A Service Factory is provided to easily add services to the bus.
Several design decisions were made to keep things as simple as possible:

- Service methods must be accept only a single argument, which is the message.
- A dispatcher can be injected into the service by creating a constructor that accepts an action to publish a message.

These design decisions was to force services to be as decoupled as possible and to rely on message passing as the 
mechanism for coordination/communication.