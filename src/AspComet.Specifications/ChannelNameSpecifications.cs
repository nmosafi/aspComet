using System;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace AspComet.Specifications
{
    // These specs are attempting to capture the rules in section 2.2 of 
    // http://svn.cometd.org/trunk/bayeux/bayeux.html
    public static class ChannelNameSpecifications
    {
        public class When_creating_a_channel_name
        {
            [TestFixture]
            public class And_the_name_does_not_start_with_slash
            {
                [Test]
                public void Argument_exception_is_thrown()
                {
                    Assert.Throws<ArgumentException>(() => ChannelName.From("foo"));
                }
            }

            [TestFixture]
            public class And_the_name_contains_wildcards
            {
                [Test]
                public void Argument_exception_is_thrown()
                {
                    Assert.Throws<ArgumentException>(() => ChannelName.From("/foo/*"));
                }
            }
        }

        public class When_matching_channels
        {
            [TestFixture]
            public class And_a_wildcard_is_not_at_the_end
            {
                [Test]
                public void Argument_exception_is_thrown()
                {
                    ChannelName channelName = ChannelName.From("/foo");
                    Assert.Throws<ArgumentException>(() => channelName.Matches("/*/bar"));
                }
            }

            [TestFixture]
            public class And_the_channel_has_a_single_segment
            {
                private ChannelName channelName;

                [SetUp]
                public void Setup()
                {
                    this.channelName = ChannelName.From("/foo");
                }

                [Test]
                public void The_same_string_matches()
                {
                    Assert.That(channelName.Matches("/foo"));
                }

                [Test]
                public void A_single_wildcard_matches()
                {
                    Assert.That(channelName.Matches("/*"));
                }

                [Test]
                public void A_double_wildcard_matches()
                {
                    Assert.That(channelName.Matches("/**"));
                }

                [Test]
                public void A_different_segment_does_not_match()
                {
                    Assert.That(channelName.Matches("/bar"), Is.False);
                }

                [Test]
                public void The_same_segment_followed_by_another_segment_does_not_match()
                {
                    Assert.That(channelName.Matches("/foo/bar"), Is.False);
                }

                [Test]
                public void The_same_segment_followed_by_wildcard_does_not_match()
                {
                    Assert.That(channelName.Matches("/foo/*"), Is.False);
                }
            }

            [TestFixture]
            public class And_the_channel_has_two_segments
            {
                private ChannelName channelName;

                [SetUp]
                public void Setup()
                {
                    this.channelName = ChannelName.From("/foo/bar");
                }

                [Test]
                public void The_same_string_matches()
                {
                    Assert.That(channelName.Matches("/foo/bar"));
                }

                [Test]
                public void A_different_second_segment_does_not_match()
                {
                    Assert.That(channelName.Matches("/foo/boo"), Is.False);
                }

                [Test]
                public void A_single_wildcard_on_second_segment_matches()
                {
                    Assert.That(channelName.Matches("/foo/*"));
                }

                [Test]
                public void A_double_wildcard_on_second_segment_matches()
                {
                    Assert.That(channelName.Matches("/foo/**"));
                }

                [Test]
                public void A_single_wildcard_on_first_segment_does_not_match()
                {
                    Assert.That(channelName.Matches("/*"), Is.False);
                }

                [Test]
                public void A_double_wildcard_on_first_segment_matches()
                {
                    Assert.That(channelName.Matches("/**"));
                }

            }
        }
    }
}

// ReSharper restore InconsistentNaming
