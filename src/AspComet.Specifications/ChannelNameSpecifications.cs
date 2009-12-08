// ReSharper disable InconsistentNaming
//
// These specs are attempting to capture the rules in section 2.2 of 
// http://svn.cometd.org/trunk/bayeux/bayeux.html
//

using System;
using Machine.Specifications;

namespace AspComet.Specifications
{
    [Subject("Channel names")]
    public class when_creating_one_which_does_not_start_with_slash
    {
        static Exception exception;

        Because of =()=> exception = Catch.Exception(() => ChannelName.From("foo"));

        It should_fail_due_to_invalid_argument =()=> exception.ShouldBeOfType(typeof(ArgumentException));
    }

    [Subject("Channel names")]
    public class when_creating_one_which_contains_wildcards
    {
        static Exception exception;

        Because of =()=> exception = Catch.Exception(() => ChannelName.From("/foo/*"));

        It should_fail_due_to_invalid_argument =()=> exception.ShouldBeOfType(typeof(ArgumentException));
    }

    [Subject("Channel names")]
    public class when_matching_channels_and_a_wildcard_is_there_but_not_at_the_end
    {
        static Exception exception;
        static ChannelName channelName;

        Establish context =()=> channelName = ChannelName.From("/foo");

        Because of =()=> exception = Catch.Exception(() => channelName.Matches("/*/bar"));

        It should_fail_due_to_invalid_argument =()=> exception.ShouldBeOfType(typeof(ArgumentException));
    }

    [Subject("Channel names")]
    public class when_matching_channels_and_the_channel_has_a_single_segment
    {
        static ChannelName channelName;

        Establish context =()=> channelName = ChannelName.From("/foo");

        It should_match_with_the_same_string =()=> channelName.ShouldMatch("/foo");
        It should_match_single_wildcard =()=> channelName.ShouldMatch("/*");
        It should_match_double_wildcard =()=> channelName.ShouldMatch("/**");
        It should_not_match_different_segment =()=> channelName.ShouldNotMatch("/bar");
        It should_not_match_same_segment_followed_by_another_segment =()=> channelName.ShouldNotMatch("/foo/bar");
        It should_not_match_same_segment_followed_by_wildcard =()=> channelName.ShouldNotMatch("/foo/*");
    }

    [Subject("Channel names")]
    public class when_matching_channels_and_the_channel_has_two_segments
    {
        static ChannelName channelName;

        Establish context = () => channelName = ChannelName.From("/foo/bar");
        
        It should_match_the_same_string = () => channelName.ShouldMatch("/foo/bar");
        It should_not_match_channelname_with_a_different_second_segment =()=> channelName.ShouldNotMatch("/foo/boo");
        It should_match_a_single_wildcard_on_second_segment =()=> channelName.ShouldMatch("/foo/*");
        It should_match_a_double_wildcard_on_second_segment =()=> channelName.ShouldMatch("/foo/**");
        It should_not_match_a_single_wildcard_on_first_segment = () => channelName.ShouldNotMatch("/*");
        It should_match_a_double_wildcard_on_first_segment = () => channelName.ShouldMatch("/**");
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
