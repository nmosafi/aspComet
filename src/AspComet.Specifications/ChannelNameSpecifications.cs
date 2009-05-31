// ReSharper disable InconsistentNaming
//
// These specs are attempting to capture the rules in section 2.2 of 
// http://svn.cometd.org/trunk/bayeux/bayeux.html
//

using System;
using NUnit.Framework;

using SpecUnit;

namespace AspComet.Specifications
{
    [Concern("Channel names")]
    public class when_creating_a_channel_name_which_does_not_start_with_slash : ContextSpecification
    {
        private Exception exception;

        protected override void  Because()
        {
            exception = ((MethodThatThrows) (() => ChannelName.From("foo"))).GetException();
        }

        [Observation]
        public void should_fail_due_to_invalid_argument()
        {
            exception.ShouldBeOfType(typeof(ArgumentException));
        }
    }

    [Concern("Channel names")]
    public class when_creating_a_channel_name_which_contains_wildcards : ContextSpecification
    {
        private Exception exception;

        protected override void Because()
        {
            exception = ((MethodThatThrows) (() => ChannelName.From("/foo/*"))).GetException();
        }

        [Observation]
        public void should_fail_due_to_invalid_argument()
        {
            exception.ShouldBeOfType(typeof(ArgumentException));
        }
    }

    [Concern("Channel names")]
    public class when_matching_channels_and_a_wildcard_is_there_but_not_at_the_end : ContextSpecification
    {
        private Exception exception;
        private ChannelName channelName;

        protected override void Context()
        {
            channelName = ChannelName.From("/foo");
        }

        protected override void Because()
        {
            exception = ((MethodThatThrows) (() => channelName.Matches("/*/bar"))).GetException();
        }

        [Observation]
        public void should_fail_due_to_invalid_argument()
        {
            exception.ShouldBeOfType(typeof(ArgumentException));
        }
    }

    [Concern("Channel names")]
    public class when_matching_channels_and_the_channel_has_a_single_segment : ContextSpecification
    {
        private ChannelName channelName;

        protected override void Context()
        {
            channelName = ChannelName.From("/foo");
        }

        [Observation]
        public void should_match_with_the_same_string()
        {
            channelName.ShouldMatch("/foo");
        }

        [Test]
        public void should_match_single_wildcard()
        {
            channelName.ShouldMatch("/*");
        }

        [Test]
        public void should_match_double_wildcard()
        {
            channelName.ShouldMatch("/**");
        }

        [Test]
        public void should_not_match_different_segment()
        {
            channelName.ShouldNotMatch("/bar");
        }

        [Test]
        public void should_not_match_same_segment_followed_by_another_segment()
        {
            channelName.ShouldNotMatch("/foo/bar");
        }

        [Test]
        public void should_not_match_same_segment_followed_by_wildcard()
        {
            channelName.ShouldNotMatch("/foo/*");
        }
    }

    [Concern("Channel names")]
    public class when_matching_channels_and_the_channel_has_two_segments : ContextSpecification
    {
        private ChannelName channelName;

        protected override void Context()
        {
            channelName = ChannelName.From("/foo/bar");
        }

        [Test]
        public void should_match_the_same_string()
        {
            channelName.ShouldMatch("/foo/bar");
        }

        [Test]
        public void should_not_match_channelname_with_a_different_second_segment()
        {
            channelName.ShouldNotMatch("/foo/boo");
        }

        [Test]
        public void A_single_wildcard_on_second_segment_matches()
        {
            channelName.ShouldMatch("/foo/*");
        }

        [Test]
        public void A_double_wildcard_on_second_segment_matches()
        {
            channelName.ShouldMatch("/foo/**");
        }

        [Test]
        public void A_single_wildcard_on_first_segment_does_not_match()
        {
            channelName.ShouldNotMatch("/*");
        }

        [Test]
        public void A_double_wildcard_on_first_segment_matches()
        {
            channelName.ShouldMatch("/**");
        }
    }

    public static class ChannelNameExtensions
    {
        public static void ShouldMatch(this ChannelName channelName, string match)
        {
            channelName.Matches(match).ShouldBeTrue();
        }

        public static void ShouldNotMatch(this ChannelName channelName, string match)
        {
            channelName.Matches(match).ShouldBeFalse();
        }
    }
}
