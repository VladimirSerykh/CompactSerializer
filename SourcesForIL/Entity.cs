using System;
using System.Collections.Generic;
using ProtoBuf;

namespace SourcesForIL
{
    [Serializable]
    [ProtoContract]
    public class Entity
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public string ShortName { get; set; }

        [ProtoMember(4)]
        public string Description { get; set; }

        [ProtoMember(5)]
        public char Label { get; set; }

        [ProtoMember(6)]
        public byte Age { get; set; }

        [ProtoMember(7)]
        public int Index { get; set; }

        [ProtoMember(8)]
        public float Weigth { get; set; }

        [ProtoMember(9)]
        public double Rating { get; set; }

        [ProtoMember(10)]
        public decimal Price { get; set; }

        [ProtoMember(11)]
        public bool IsVisible { get; set; }

        [ProtoMember(12)]
        public short ShortIndex { get; set; }

        [ProtoMember(13)]
        public long LongIndex { get; set; }

        [ProtoMember(14)]
        public uint UnsignedIndex { get; set; }

        [ProtoMember(15)]
        public ushort ShortUnsignedIndex { get; set; }

        [ProtoMember(16)]
        public ulong LongUnsignedIndex { get; set; }

        [ProtoMember(17)]
        public DateTime CreatedAt { get; set; }

        [ProtoMember(18)]
        public DateTime CreatedAtUtc { get; set; }

        [ProtoMember(19)]
        public DateTime LastAccessed { get; set; }

        [ProtoMember(20)]
        public DateTimeOffset ChangedAt { get; set; }

        [ProtoMember(21)]
        public DateTimeOffset ChangedAtUtc { get; set; }

        [ProtoMember(22)]
        public int[] References { get; set; }

        [ProtoMember(23)]
        public List<short> Weeks { get; set; }

        [ProtoMember(24)]
        public bool[] BitMap { get; set; }

        [ProtoMember(25)]
        public Guid[] ChildrenIds { get; set; }

        [ProtoMember(26)]
        public DateTime[] Schedule { get; set; }

        [ProtoMember(27)]
        public DateTimeOffset[] Moments { get; set; }

        [ProtoMember(28)]
        public List<string> Tags { get; set; }

        [ProtoMember(29)]
        public decimal[] PricesHistory { get; set; }

        [ProtoMember(30)]
        public Guid? AlternativeId { get; set; }
    }
}
