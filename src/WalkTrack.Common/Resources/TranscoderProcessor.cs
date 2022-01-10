/*
    WalkTrack - A simple walk tracker that lets people define their own milestones

    Copyright (C) 2022  Nathan Mentley
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace WalkTrack.Common.Resources;

/// <summary>
/// </summary>
internal sealed class TranscoderProcessor: ITranscoderProcessor
{
    private readonly IEnumerable<IWireTranscoder> _wireTranscoders;
    private readonly IEnumerable<IPersistTranscoder> _persistTranscoders;

    public TranscoderProcessor(
        IEnumerable<IWireTranscoder> wireTranscoders,
        IEnumerable<IPersistTranscoder> persistTranscoders
    )
    {
        _wireTranscoders = wireTranscoders ??
            throw new ArgumentNullException(nameof(wireTranscoders));

        _persistTranscoders = persistTranscoders ??
            throw new ArgumentNullException(nameof(persistTranscoders));
    }

    public async Task<T> Decode<T>(
        WalkTrackMediaType mediaType,
        Stream stream,
        bool forPersistence,
        CancellationToken cancellationToken
    )
    {
        if (mediaType is null)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        cancellationToken.ThrowIfCancellationRequested();

        ITranscoder? transcoder = GetTranscoder<T>(mediaType, forPersistence);

        if (transcoder is not null)
        {
            object result = await transcoder.Decode(stream, cancellationToken);

            if (result is T typedResult)
            {
                return typedResult;
            }

            throw new InvalidOperationException();
        }

        throw new NotSupportedException();
    }

    public async Task Encode<T>(
        WalkTrackMediaType mediaType,
        T instance,
        Stream stream,
        bool forPersistence,
        CancellationToken cancellationToken
    )
    {
        if (mediaType is null)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        cancellationToken.ThrowIfCancellationRequested();

        ITranscoder? transcoder = GetTranscoder<T>(mediaType, forPersistence);

        if (transcoder is not null)
        {
            await transcoder.Encode(instance, stream, cancellationToken);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private ITranscoder? GetTranscoder<T>(WalkTrackMediaType mediaType, bool forPersistence)
    {
        if (forPersistence)
        {
            return _persistTranscoders
                .FirstOrDefault(
                    transcoder =>
                        transcoder.CanHandle(mediaType) &&
                        transcoder.CanHandle(typeof(T))
                );
        }

        return _wireTranscoders
            .FirstOrDefault(
                transcoder =>
                    transcoder.CanHandle(mediaType) &&
                    transcoder.CanHandle(typeof(T))
            );
    }
}